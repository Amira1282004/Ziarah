"use strict";

(function () {
    const root = document.getElementById("verifyPageRoot");
    const boxes = document.querySelectorAll("#codeInputs .code-box");
    const hidden = document.getElementById("codeHidden");
    const form = document.getElementById("verifyForm");
    const timerEl = document.getElementById("timer");

    function syncHidden() {
        if (!hidden) return;
        hidden.value = Array.from(boxes).map(function (b) { return b.value.replace(/\D/g, ""); }).join("");
    }

    boxes.forEach(function (box, idx) {
        box.addEventListener("input", function () {
            this.value = this.value.replace(/\D/g, "").slice(0, 1);
            if (this.value && idx < boxes.length - 1) {
                boxes[idx + 1].focus();
            }
            syncHidden();
        });
        box.addEventListener("keydown", function (ev) {
            if (ev.key === "Backspace" && !this.value && idx > 0) {
                boxes[idx - 1].focus();
            }
        });
    });

    if (form) {
        form.addEventListener("submit", function () {
            syncHidden();
        });
    }

    if (timerEl && root) {
        var expiresMs = parseInt(root.getAttribute("data-expires-ms") || "0", 10);
        function tick() {
            var now = Date.now();
            var left = Math.max(0, expiresMs - now);
            var totalSec = Math.floor(left / 1000);
            var m = Math.floor(totalSec / 60);
            var s = totalSec % 60;
            timerEl.textContent = m + ":" + (s < 10 ? "0" : "") + s;
        }
        tick();
        setInterval(tick, 1000);
    }

    if (boxes.length) {
        boxes[0].focus();
    }
})();
