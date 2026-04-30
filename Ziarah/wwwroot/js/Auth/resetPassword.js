"use strict";

(function () {
    function bindToggle(btnId, inputId) {
        const btn   = document.getElementById(btnId);
        const input = document.getElementById(inputId);
        if (!btn || !input) return;
        btn.addEventListener("click", function () {
            if (input.type === "password") {
                input.type = "text";
                this.classList.replace("fa-eye", "fa-eye-slash");
            } else {
                input.type = "password";
                this.classList.replace("fa-eye-slash", "fa-eye");
            }
        });
    }

    bindToggle("togglePassword",        "password");
    bindToggle("toggleConfirmPassword", "confirmPassword");

    const pwdInput      = document.getElementById("password");
    const strengthDiv   = document.getElementById("passwordStrength");
    const strengthFill  = document.getElementById("strengthFill");
    const strengthLabel = document.getElementById("strengthLabel");

    const levels = [
        { label: "ضعيفة جداً", color: "#e72c2c", width: "20%" },
        { label: "ضعيفة",      color: "#e6a817", width: "40%" },
        { label: "متوسطة",     color: "#f59e0b", width: "60%" },
        { label: "جيدة",       color: "#22c55e", width: "80%" },
        { label: "قوية",       color: "#16a34a", width: "100%" }
    ];

    if (pwdInput && strengthDiv) {
        pwdInput.addEventListener("input", function () {
            const val = this.value;
            if (!val) { strengthDiv.style.display = "none"; return; }
            strengthDiv.style.display = "block";
            let score = 0;
            if (val.length >= 8)              score++;
            if (/[A-Z]/.test(val))            score++;
            if (/[a-z]/.test(val))            score++;
            if (/\d/.test(val))               score++;
            if (/[!@#$%^&*]/.test(val))       score++;
            const lvl = levels[score - 1] || levels[0];
            strengthFill.style.width      = lvl.width;
            strengthFill.style.background = lvl.color;
            strengthLabel.textContent     = lvl.label;
            strengthLabel.style.color     = lvl.color;
        });
    }

    // ── التحقق من تطابق كلمتي المرور قبل الإرسال ───────────────────────────
    const form = document.getElementById("resetForm");
    if (form) {
        form.addEventListener("submit", function (e) {
            const p1 = document.getElementById("password")?.value;
            const p2 = document.getElementById("confirmPassword")?.value;
            if (p1 !== p2) {
                e.preventDefault();
                const overlay = document.getElementById("errorPopupOverlay");
                if (overlay) {
                    const list = overlay.querySelector(".popup-error-list");
                    if (list) list.innerHTML = '<li><i class="fa-solid fa-circle-dot"></i> كلمتا المرور غير متطابقتين.</li>';
                    overlay.classList.add("active");
                }
            }
        });
    }
})();