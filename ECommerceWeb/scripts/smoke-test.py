#!/usr/bin/env python3
"""Exercise the storefront and session cart against a running local app."""

import re
import sys
from html.parser import HTMLParser
from http.cookiejar import CookieJar
from urllib.error import HTTPError
from urllib.parse import urlencode, urljoin, urlparse
from urllib.request import HTTPCookieProcessor, Request, build_opener


class Page(HTMLParser):
    def __init__(self, html):
        super().__init__()
        self.forms, self.links, self.images, self.words = [], [], [], []
        self.form = None
        self.feed(html)

    def handle_starttag(self, tag, attrs):
        attrs = dict(attrs)
        if tag == "form":
            self.form = {"action": attrs.get("action", ""), "inputs": {}}
            self.forms.append(self.form)
        elif tag == "input" and self.form is not None and "name" in attrs:
            self.form["inputs"][attrs["name"]] = attrs.get("value", "")
            if attrs["name"] == "quantity":
                self.form["quantity_attrs"] = attrs
        elif tag == "a":
            self.links.append(attrs.get("href", ""))
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

    def handler(self, name):
        return next(form for form in self.forms if f"handler={name}" in form["action"])


base = (sys.argv[1] if len(sys.argv) > 1 else "http://localhost:5187").rstrip("/")
client = build_opener(HTTPCookieProcessor(CookieJar()))


def fetch(path, fields=None, expected=200):
    request = Request(urljoin(base + "/", path),
                      data=urlencode(fields).encode() if fields is not None else None)
    try:
        response = client.open(request, timeout=15)
    except HTTPError as error:
        response = error
    assert response.status == expected, f"{request.get_method()} {path}: {response.status}, expected {expected}"
    html = response.read().decode("utf-8")
    return Page(html), response.geturl()


def submit(form, **values):
    fields = form["inputs"] | values
    assert fields.get("__RequestVerificationToken"), "Missing antiforgery token"
    page, url = fetch(form["action"], fields)
    assert urlparse(url).path == "/Cart", f"Cart redirect missing: {url}"
    return page


home, _ = fetch("/")
detail_paths = list(dict.fromkeys(link for link in home.links if re.fullmatch(r"/Product/\d+", link)))
assert len(detail_paths) == 6, f"Expected six phone detail links: {detail_paths}"
assert not any(emoji in home.text for emoji in ("👕", "👟", "🎒", "🎧", "⌚", "💻", "🛍", "📱", "🛒"))
assert len(home.images) >= 6
for image in home.images:
    assert image.get("alt", "").strip(), f"Missing image alt: {image}"
    with client.open(urljoin(base, image["src"]), timeout=15) as response:
        assert response.status == 200 and response.headers.get_content_type().startswith("image/")

details = []
for path in detail_paths:
    detail, _ = fetch(path)
    add = detail.handler("Add")
    assert int(add["inputs"]["id"]) == int(path.rsplit("/", 1)[1])
    assert add["inputs"].get("__RequestVerificationToken")
    assert detail.images and "Thông số kỹ thuật" in detail.text
    details.append(detail)
fetch("/Product/999999", expected=404)

first = details[0]
price_match = re.search(r"(\d{1,3}(?:\.\d{3})+)\s*[đ₫]", first.text)
assert price_match, "Could not read the first phone price"
price = int(price_match[1].replace(".", ""))
add = first.handler("Add")
fetch(add["action"], {"id": add["inputs"]["id"]}, expected=400)
fetch(add["action"], add["inputs"] | {"id": "999999"}, expected=404)

cart = submit(add)
update = cart.handler("Update")
assert update["inputs"]["quantity"] == "1"
assert "requestSubmit" in update["quantity_attrs"].get("onchange", "")
assert "Cập nhật" not in cart.text, "The manual update button returned"

for quantity, expected in ((3, 3), (100, 99)):
    cart = submit(cart.handler("Update"), quantity=quantity)
    assert cart.handler("Update")["inputs"]["quantity"] == str(expected)
    total = f"{price * expected:,}".replace(",", ".")
    assert len(re.findall(re.escape(total) + r"\s*[đ₫]", cart.text)) >= 2, f"Subtotal/total did not both update to {total}"

cart = submit(cart.handler("Update"), quantity=0)
assert not any("handler=Update" in form["action"] for form in cart.forms)
cart = submit(add)
cart = submit(cart.handler("Remove"))
assert not any("handler=Update" in form["action"] for form in cart.forms)
cart = submit(add)
cart = submit(cart.handler("Checkout"))
assert "Đặt hàng giả lập thành công!" in cart.text
assert not any("handler=Update" in form["action"] for form in cart.forms)
print("PASS: phone pages/images, invalid IDs, antiforgery, add, automatic quantity, price totals, removal, demo checkout.")
