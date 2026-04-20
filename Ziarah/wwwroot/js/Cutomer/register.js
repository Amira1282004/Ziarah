const hasInsurance = document.getElementById("hasInsurance");
const insuranceUpload = document.getElementById("insuranceUpload");

hasInsurance.addEventListener("change", function () {
    if (this.checked) {
        insuranceUpload.style.display = "block";
    } else {
        insuranceUpload.style.display = "none";
    }
});

let selectedRole = "patient";

const roleButtons = document.querySelectorAll(".role-btn");
const certificateField = document.getElementById("certificateField");
const separateNameFields = document.getElementById("separateNameFields");
const proofField = document.getElementById("proofField");
const insuranceField = document.getElementById("insuranceField");
const specializationField = document.getElementById("specializationField");

roleButtons.forEach(btn => {
    btn.addEventListener("click", () => {

        roleButtons.forEach(b => b.classList.remove("active"));
        btn.classList.add("active");

        selectedRole = btn.dataset.role;

        const certificateLabel = document.querySelector("#certificateField label");

        if(selectedRole === "doctor"){
            certificateLabel.innerText = "شهادة الطب";
        }
        else if(selectedRole === "nurse"){
            certificateLabel.innerText = "شهادة التمريض";
        }

        if (selectedRole === "patient") {
            fullNameField.style.display = "block";
            separateNameFields.style.display = "none";

            insuranceField.style.display = "block";
            proofField.style.display = "block";

            specializationField.style.display = "none";
            certificateField.style.display = "none";
        } 
        else if (selectedRole === "doctor") {
            fullNameField.style.display = "none";
            separateNameFields.style.display = "flex";

            insuranceField.style.display = "none";
            proofField.style.display = "block";

            specializationField.style.display = "block";
            certificateField.style.display = "block";
        } 
        else if (selectedRole === "nurse") {
            fullNameField.style.display = "none";
            separateNameFields.style.display = "flex";

            insuranceField.style.display = "none";
            proofField.style.display = "block";

            specializationField.style.display = "none";
            certificateField.style.display = "block";
        }

        updateRequiredFields();
    });
});
roleButtons.forEach(btn => {
    btn.addEventListener("click", () => {
        roleButtons.forEach(b => b.classList.remove("active"));
        btn.classList.add("active");

        selectedRole = btn.dataset.role;
        document.getElementById("RoleInput").value = selectedRole;

        const certificateLabel = document.querySelector("#certificateField label");
        if (selectedRole === "doctor") {
            certificateLabel.innerText = "شهادة الطب";
        } else if (selectedRole === "nurse") {
            certificateLabel.innerText = "شهادة التمريض";
        }

        if (selectedRole === "patient") {
            separateNameFields.style.display = "flex";
            insuranceField.style.display = "block";
            proofField.style.display = "block";
            specializationField.style.display = "none";
            certificateField.style.display = "none";
        } else if (selectedRole === "doctor") {
            separateNameFields.style.display = "flex";
            insuranceField.style.display = "none";
            proofField.style.display = "block";
            specializationField.style.display = "block";
            certificateField.style.display = "block";
        } else if (selectedRole === "nurse") {
            separateNameFields.style.display = "flex";
            insuranceField.style.display = "none";
            proofField.style.display = "block";
            specializationField.style.display = "none";
            certificateField.style.display = "block";
        }
    });
});


const togglePassword = document.getElementById("togglePassword");
const passwordInput = document.getElementById("password");

togglePassword.addEventListener("click", function () {

    if (passwordInput.type === "password") {
        passwordInput.type = "text";
        this.classList.remove("fa-eye");
        this.classList.add("fa-eye-slash");
    } else {
        passwordInput.type = "password";
        this.classList.remove("fa-eye-slash");
        this.classList.add("fa-eye");
    }

});
const password = document.getElementById("password");

password.addEventListener("keyup", function () {
    const value = password.value;

    const isLength = value.length >= 8;
    const hasUpper = /[A-Z]/.test(value);
    const hasLower = /[a-z]/.test(value);
    const hasNumber = /[0-9]/.test(value);
    const hasSpecial = /[!@#$%^&*]/.test(value);

    if (isLength && hasUpper && hasLower && hasNumber && hasSpecial) {
        password.classList.remove("invalid-password");
        password.classList.add("valid-password");
    } else {
        password.classList.remove("valid-password");
        password.classList.add("invalid-password");
    }
});



document.getElementById("registerForm").addEventListener("submit", function(e){
    
    if(!acceptTerms.checked){
        e.preventDefault();
        alert("يجب الموافقة على سياسة الخصوصية وشروط الاستخدام");
        return;
    }

});


function handleFileInput(input, labelId, maxFiles) {
    const label = document.getElementById(labelId);
    const originalText = label.innerText; 

    input.addEventListener("change", function() {
        const files = Array.from(input.files);

        if (files.length > maxFiles) {
            alert(`يمكنك رفع ${maxFiles} ملف${maxFiles > 1 ? 'ات' : ''} فقط.`);
            input.value = ""; 
            label.innerText = originalText;
            return;
        }

        for (let file of files) {
            if (!file.type.startsWith("image/")) {
                alert("يمكنك رفع صور فقط (jpg, jpeg, png).");
                input.value = "";
                label.innerText = originalText;
                return;
            }
        }

        const fileNames = files.map(f => f.name).join(", ");
        label.innerText = `تم رفع: ${fileNames}`;
    });
}

handleFileInput(document.getElementById("document"), "documentLabel", 2);
handleFileInput(document.getElementById("certificate"), "certificateLabel", 1);

const insuranceInput = document.getElementById("insuranceDoc");

function handleFileInputSingle(input, labelId) {
    const label = document.getElementById(labelId);
    const originalText = label.innerText;

    input.addEventListener("change", function() {
        const files = Array.from(input.files);

        if (files.length === 0) {
            label.innerText = originalText;
            return;
        }

        for (let file of files) {
            if (!file.type.startsWith("image/")) {
                alert("يمكنك رفع صور فقط (jpg, jpeg, png).");
                input.value = "";
                label.innerText = originalText;
                return;
            }
        }

        label.innerText = `تم رفع: ${files[0].name}`;
    });
}

handleFileInputSingle(insuranceInput, "insuranceLabel");