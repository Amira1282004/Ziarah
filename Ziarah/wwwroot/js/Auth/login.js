"use strict";

(function () {
    const toggleBtn = document.getElementById("toggleLoginPassword");
    const pwdInput  = document.getElementById("loginPassword");

    if (toggleBtn && pwdInput) {
        toggleBtn.addEventListener("click", function () {
            if (pwdInput.type === "password") {
                pwdInput.type = "text";
                this.classList.replace("fa-eye", "fa-eye-slash");
            } else {
                pwdInput.type = "password";
                this.classList.replace("fa-eye-slash", "fa-eye");
            }
        });
    }

    const form = document.getElementById("loginForm");
    if (form) {
        form.addEventListener("submit", function (e) {
            const email    = document.getElementById("loginEmail")?.value.trim();
            const password = pwdInput?.value.trim();

            if (!email || !password) {
                e.preventDefault();
                const overlay = document.getElementById("errorPopupOverlay");
                if (overlay) {
                    const list = overlay.querySelector(".popup-error-list");
                    if (list) {
                        list.innerHTML = '<li><i class="fa-solid fa-circle-dot"></i> يرجى تعبئة جميع الحقول المطلوبة.</li>';
                    }
                    overlay.classList.add("active");
                }
            }
        });
    }
})();