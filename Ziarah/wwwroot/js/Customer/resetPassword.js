const toggleConfromPassword = document.getElementById("toggleConfromPassword");
const passwordConfromInput = document.getElementById("confromPassword");

toggleConfromPassword.addEventListener("click", function () {

    if (passwordConfromInput.type === "password") {
        passwordConfromInput.type = "text";
        this.classList.remove("fa-eye");
        this.classList.add("fa-eye-slash");
    } else {
        passwordConfromInput.type = "password";
        this.classList.remove("fa-eye-slash");
        this.classList.add("fa-eye");
    }

});
