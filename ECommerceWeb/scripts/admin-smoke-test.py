#!/usr/bin/env python3
"""Check admin authentication against a running app; supply credentials via env."""

import os
import sys
from html.parser import HTMLParser
from http.cookiejar import CookieJar
from urllib.error import HTTPError
from urllib.parse import parse_qs, urlencode, urljoin, urlparse
from urllib.request import HTTPCookieProcessor, HTTPRedirectHandler, Request, build_opener


class Page(HTMLParser):
    def __init__(self, html):
        super().__init__()
        self.forms, self.images, self.words = [], [], []
        self.form = None
        self.feed(html)

    def handle_starttag(self, tag, attrs):
        attrs = dict(attrs)
        if tag == "form":
            self.form = {"action": attrs.get("action", ""), "inputs": {}}
            self.forms.append(self.form)
        elif tag == "input" and self.form is not None and "name" in attrs:
            self.form["inputs"][attrs["name"]] = attrs.get("value", "")
        elif tag == "img":
            self.images.append(attrs)

    def handle_endtag(self, tag):
        if tag == "form":
            self.form = None

    def handle_data(self, data):
        self.words.append(data.strip())

    @property
    def text(self):
        return " ".join(self.words)


class NoRedirect(HTTPRedirectHandler):
    def redirect_request(self, request, response, code, message, headers, newurl):
        return None


base = (sys.argv[1] if len(sys.argv) > 1 else "http://localhost:5187").rstrip("/")
username = os.environ.get("MOBI_TEST_ADMIN_USERNAME", "admin")
password = os.environ.get("MOBI_TEST_ADMIN_PASSWORD")
if not password:
    sys.exit("Set MOBI_TEST_ADMIN_PASSWORD to the configured admin password.")
cookies = CookieJar()
client = build_opener(HTTPCookieProcessor(cookies))
no_redirect = build_opener(HTTPCookieProcessor(cookies), NoRedirect())


def fetch(path, fields=None, expected=200, follow=True):
    request = Request(urljoin(base + "/", path),
                      data=urlencode(fields).encode() if fields is not None else None)
    try:
        response = (client if follow else no_redirect).open(request, timeout=15)
    except HTTPError as error:
        response = error
    with response:
        page = Page(response.read().decode("utf-8"))
    assert response.status in (expected if isinstance(expected, tuple) else (expected,)), f"{request.get_method()} {path}: {response.status}, expected {expected}"
    return page, response


def login_form(return_url="/admin"):
    page, response = fetch("/admin/login?" + urlencode({"ReturnUrl": return_url}))
    form = next(form for form in page.forms if "Password" in form["inputs"])
    form["action"] = form["action"] or response.geturl()
    assert {"Username", "Password", "ReturnUrl", "__RequestVerificationToken"} <= form["inputs"].keys()
    assert form["inputs"]["__RequestVerificationToken"], "Login antiforgery token missing"
    return form


def assert_admin():
    page, response = fetch("/admin")
    assert urlparse(response.geturl()).path.rstrip("/").lower() == "/admin", "Authentication did not protect the dashboard"
    assert "no-store" in response.headers.get("Cache-Control", "").lower(), "Dashboard may be cached"
    assert len([image for image in page.images if "/images/products/" in image.get("src", "")]) == 6
    assert "iPhone 16" in page.text and "Galaxy S25" in page.text, "Phone catalog missing from dashboard"
    _, uppercase = fetch("/Admin")
    assert urlparse(uppercase.geturl()).path.lower() == "/admin", "Mixed-case admin URL lost authorization"
    return page


def logout():
    page = assert_admin()
    form = next(form for form in page.forms if urlparse(form["action"]).path.lower() == "/admin/logout")
    assert form["inputs"].get("__RequestVerificationToken"), "Logout antiforgery token missing"
    fetch(form["action"], form["inputs"], expected=302, follow=False)
    _, response = fetch("/admin", expected=302, follow=False)
    assert urlparse(response.headers["Location"]).path.lower() == "/admin/login", "Logout retained authorization"


page, response = fetch("/admin", expected=302, follow=False)
target = urlparse(response.headers["Location"])
assert target.path.lower() == "/admin/login"
assert parse_qs(target.query).get("ReturnUrl") == ["/admin"]
assert "iPhone 16" not in page.text and "Galaxy S25" not in page.text, "Anonymous response leaked the dashboard"

form = login_form()
fetch(form["action"], {"Username": username, "Password": password, "ReturnUrl": "/admin"}, expected=400)
wrong, _ = fetch(form["action"], form["inputs"] | {"Username": username, "Password": password + "-incorrect"})
assert "Tên đăng nhập hoặc mật khẩu không đúng." in wrong.text, "Generic login error missing"
assert not any(form["inputs"].get("Password") for form in wrong.forms), "Login echoed the password"
assert password + "-incorrect" not in wrong.text, "Login displayed the password"
fetch("/admin", expected=302, follow=False)

form = login_form()
_, response = fetch(form["action"], form["inputs"] | {"Username": username, "Password": password}, expected=302, follow=False)
assert urlparse(response.headers["Location"]).path.lower() == "/admin"
auth_headers = response.headers.get_all("Set-Cookie", [])
assert any(header.startswith("Mobi.Admin=") and "httponly" in header.lower() and "samesite=lax" in header.lower() for header in auth_headers), "Auth cookie protections missing"
assert_admin()
request = Request(urljoin(base, "/admin/logout"))
try:
    response = no_redirect.open(request, timeout=15)
except HTTPError as error:
    response = error
with response:
    response.read()
assert response.status == 405, f"GET logout allowed an unsupported method: {response.status}"
page = assert_admin()
logout_form = next(form for form in page.forms if "/admin/logout" in form["action"].lower())
fetch(logout_form["action"], {}, expected=400)
logout()

for hostile in ("https://example.com/", "//example.com/"):
    form = login_form(hostile)
    _, response = fetch(form["action"], form["inputs"] | {"Username": username, "Password": password, "ReturnUrl": hostile}, expected=302, follow=False)
    location = urlparse(response.headers["Location"])
    assert not location.netloc and location.path.lower() == "/admin", "Login allowed an external redirect"
    logout()

form = login_form()
for _ in range(11):
    wrong, _ = fetch(form["action"], form["inputs"] | {"Username": username, "Password": password + "-incorrect"}, expected=200, follow=False)
    assert "Tên đăng nhập hoặc mật khẩu không đúng." in wrong.text, "Repeated login attempts lost the credential error"
fetch(form["action"], form["inputs"] | {"Username": username, "Password": password}, expected=302, follow=False)
assert_admin()
logout()

print("PASS: admin authorization, login/logout antiforgery, invalid credentials, cookie protection, POST-only logout, local return URLs, six-phone dashboard, cache protection, repeated login without throttling.")
