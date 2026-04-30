"use strict";

window.sendMessage = function (e) {
    e.preventDefault();

    const btn = document.getElementById("sendBtn");
    if (!btn) return;

    btn.disabled = true;
    btn.innerHTML = '<i class="fa-solid fa-spinner fa-spin"></i> جاري الإرسال...';

    setTimeout(() => {
        const form    = document.getElementById("contactForm");
        const success = document.getElementById("formSuccess");

        if (form)    form.style.display    = "none";
        if (success) success.classList.add("show");

        btn.disabled = false;
        btn.innerHTML = '<i class="fa-solid fa-paper-plane"></i> إرسال الرسالة';
    }, 1800);
};

window.resetForm = function () {
    const form    = document.getElementById("contactForm");
    const success = document.getElementById("formSuccess");

    if (form) {
        form.reset();
        form.style.display = "flex";
    }
    if (success) success.classList.remove("show");
};

window.toggleFaq = function (btn) {
    const item   = btn.closest(".faq-item");
    const answer = item.querySelector(".faq-answer");
    const isOpen = btn.classList.contains("open");

    document.querySelectorAll(".faq-question.open").forEach(q => {
        q.classList.remove("open");
        q.closest(".faq-item").querySelector(".faq-answer").classList.remove("open");
    });

    if (!isOpen) {
        btn.classList.add("open");
        answer.classList.add("open");
    }
};