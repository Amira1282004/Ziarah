const toggleLoginPassword = document.getElementById("toggleLoginPassword");
const loginPasswordInput = document.getElementById("loginPassword");

toggleLoginPassword.addEventListener("click", function () {
    if (loginPasswordInput.type === "password") {
        loginPasswordInput.type = "text";
        this.classList.remove("fa-eye");
        this.classList.add("fa-eye-slash");
    } else {
        loginPasswordInput.type = "password";
        this.classList.remove("fa-eye-slash");
        this.classList.add("fa-eye");
    }
});

document.getElementById("loginForm").addEventListener("submit", function (e) {
    const email = document.getElementById("loginEmail").value.trim();
    const password = loginPasswordInput.value.trim();

    if (!email || !password) {
        e.preventDefault();
        alert("يرجى تعبئة جميع الحقول المطلوبة.");
        return;
    }
});