"use strict";

(function () {
    const formAlert = document.getElementById("formAlert");
    function showAlert(message, type) {
        if (!formAlert) return;
        formAlert.className = "form-alert " + (type || "error");
        formAlert.textContent = message;
        formAlert.style.display = "block";
        formAlert.scrollIntoView({ behavior: "smooth", block: "center" });
    }

    function hideAlert() {
        if (!formAlert) return;
        formAlert.style.display = "none";
        formAlert.textContent = "";
    }

    const insuranceCheckbox = document.getElementById("hasInsurance");
    const insuranceUpload = document.getElementById("insuranceUpload");
    if (insuranceCheckbox && insuranceUpload) {
        insuranceCheckbox.addEventListener("change", function () {
            hideAlert();
            insuranceUpload.style.display = this.checked ? "block" : "none";
            const insInput = document.getElementById("insuranceDoc");
            if (insInput) insInput.required = this.checked;
        });
    }

    const roleButtons = document.querySelectorAll(".role-btn");
    const roleInput = document.getElementById("RoleInput");
    const certificateField = document.getElementById("certificateField");
    const specializationField = document.getElementById("specializationField");
    const doctorPricingRow = document.getElementById("doctorPricingRow");
    const insuranceField = document.getElementById("insuranceField");
    const certInput = document.getElementById("professionalLicense");
    const certLabel = document.getElementById("certificateLabel");
    const specSelect = document.getElementById("specializationId");
    const priceInput = document.getElementById("consultationPrice");
    const expInput = document.getElementById("experienceYears");

    function applyRole(role) {
        if (roleInput) roleInput.value = role;

        if (certLabel) {
            if (role === "doctor") certLabel.innerText = "شهادة الطب / مزاولة المهنة";
            else if (role === "nurse") certLabel.innerText = "شهادة التمريض / مزاولة المهنة";
        }

        if (role === "patient") {
            if (insuranceField) insuranceField.style.display = "block";
            if (specializationField) specializationField.style.display = "none";
            if (doctorPricingRow) doctorPricingRow.style.display = "none";
            if (certificateField) certificateField.style.display = "none";
            if (certInput) { certInput.required = false; certInput.value = ""; }
            if (specSelect) { specSelect.required = false; specSelect.value = ""; }
            if (priceInput) { priceInput.required = false; priceInput.value = ""; }
            if (expInput) { expInput.required = false; expInput.value = ""; }
        } else if (role === "doctor") {
            if (insuranceField) insuranceField.style.display = "none";
            if (specializationField) specializationField.style.display = "block";
            if (doctorPricingRow) doctorPricingRow.style.display = "";
            if (certificateField) certificateField.style.display = "block";
            if (certInput) certInput.required = true;
            if (specSelect) specSelect.required = true;
            if (priceInput) priceInput.required = true;
            if (expInput) expInput.required = true;
            if (insuranceCheckbox) { insuranceCheckbox.checked = false; insuranceCheckbox.dispatchEvent(new Event("change")); }
        } else if (role === "nurse") {
            if (insuranceField) insuranceField.style.display = "none";
            if (specializationField) specializationField.style.display = "none";
            if (doctorPricingRow) doctorPricingRow.style.display = "none";
            if (certificateField) certificateField.style.display = "block";
            if (certInput) certInput.required = true;
            if (specSelect) { specSelect.required = false; specSelect.value = ""; }
            if (priceInput) { priceInput.required = false; priceInput.value = ""; }
            if (expInput) { expInput.required = false; expInput.value = ""; }
            if (insuranceCheckbox) { insuranceCheckbox.checked = false; insuranceCheckbox.dispatchEvent(new Event("change")); }
        }
    }

    roleButtons.forEach(function (btn) {
        btn.addEventListener("click", function () {
            roleButtons.forEach(function (b) { b.classList.remove("active"); });
            btn.classList.add("active");
            applyRole(btn.dataset.role || "patient");
        });
    });

    applyRole("patient");

    const togglePassword = document.getElementById("togglePassword");
    const passwordInput = document.getElementById("password");
    if (togglePassword && passwordInput) {
        togglePassword.addEventListener("click", function () {
            if (passwordInput.type === "password") {
                passwordInput.type = "text";
                this.classList.replace("fa-eye", "fa-eye-slash");
            } else {
                passwordInput.type = "password";
                this.classList.replace("fa-eye-slash", "fa-eye");
            }
        });
    }

    function bindFileLabel(inputId, labelId) {
        const input = document.getElementById(inputId);
        const label = document.getElementById(labelId);
        if (!input || !label) return;
        const original = label.innerText;
        input.addEventListener("change", function () {
            hideAlert();
            const f = input.files && input.files[0];
            if (!f) {
                label.innerText = original;
                return;
            }
            if (!f.type.startsWith("image/")) {
                showAlert("يمكن رفع صور فقط (jpg, jpeg, png, webp, gif).", "error");
                input.value = "";
                label.innerText = original;
                return;
            }
            label.innerText = "تم اختيار: " + f.name;
        });
    }

    bindFileLabel("nationalIdFront", "nationalIdFrontLabel");
    bindFileLabel("nationalIdBack", "nationalIdBackLabel");
    bindFileLabel("professionalLicense", "certificateLabel");
    bindFileLabel("insuranceDoc", "insuranceLabel");

    const form = document.getElementById("registerForm");
    const acceptTerms = document.getElementById("acceptTerms");
    if (form && acceptTerms) {
        form.addEventListener("submit", function (e) {
            hideAlert();
            if (!acceptTerms.checked) {
                e.preventDefault();
                showAlert("يرجى الموافقة على سياسة الخصوصية وشروط الاستخدام لإكمال إنشاء الحساب.", "warning");
                return;
            }
        });
    }
})();
