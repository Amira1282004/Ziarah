"use strict";

let selectedRating = 0;

function initStars() {
    const stars = document.querySelectorAll(".stars-row i");
    if (!stars.length) return;

    stars.forEach(function (star) {
        star.addEventListener("mouseenter", function () {
            const val = parseInt(this.dataset.value);
            stars.forEach(function (s) {
                s.classList.toggle("selected", parseInt(s.dataset.value) <= val);
            });
        });

        star.addEventListener("mouseleave", function () {
            stars.forEach(function (s) {
                s.classList.toggle("selected", parseInt(s.dataset.value) <= selectedRating);
            });
        });

        star.addEventListener("click", function () {
            selectedRating = parseInt(this.dataset.value);
            stars.forEach(function (s) {
                s.classList.toggle("selected", parseInt(s.dataset.value) <= selectedRating);
            });
        });
    });
}


window.submitRating = function () {
    if (selectedRating === 0) {
        showToast("يرجى اختيار تقييم بالنجوم أولاً", "warning");
        return;
    }

    const comment = document.getElementById("review")?.value.trim() || "";
    if (!comment) {
        showToast("يرجى كتابة تعليقك قبل النشر", "warning");
        return;
    }

    resetRatingForm();
    showToast("تم نشر تقييمك بنجاح ✓");
};

function resetRatingForm() {
    selectedRating = 0;
    document.querySelectorAll(".stars-row i").forEach(s => s.classList.remove("selected"));
    const review = document.getElementById("review");
    if (review) review.value = "";
}

function showToast(msg, type) {
    document.querySelectorAll(".dp-toast").forEach(t => t.remove());

    const toast = document.createElement("div");
    toast.className = "dp-toast";
    toast.textContent = msg;

    const colors = {
        success: "#128CCF",
        warning: "#e6a817",
        error:   "#e53838"
    };
    toast.style.cssText = `
        position: fixed;
        bottom: 30px;
        left: 50%;
        transform: translateX(-50%) translateY(20px);
        background: ${colors[type] || colors.success};
        color: white;
        padding: 12px 28px;
        border-radius: 30px;
        font-size: 14px;
        font-weight: 700;
        font-family: 'Cairo', sans-serif;
        box-shadow: 0 8px 24px rgba(0,0,0,0.18);
        z-index: 99999;
        opacity: 0;
        transition: all 0.3s ease;
        white-space: nowrap;
    `;

    document.body.appendChild(toast);

    requestAnimationFrame(() => {
        toast.style.opacity = "1";
        toast.style.transform = "translateX(-50%) translateY(0)";
    });

    setTimeout(() => {
        toast.style.opacity = "0";
        toast.style.transform = "translateX(-50%) translateY(20px)";
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

window.openBookingModal = function () {
    const doctorName  = document.querySelector(".doctor-info h2")?.textContent.trim()  || "";
    const doctorSpec  = document.querySelector(".specialty")?.textContent.trim()       || "";
    const doctorPrice = document.querySelector(".price-box .price")?.textContent.trim() || "";
    const doctorImg   = document.querySelector(".doctor-image img")?.src                || "";
    const doctorId    = document.body.dataset.doctorId || "";

    const nameEl  = document.querySelector(".booking-provider-name");
    const specEl  = document.querySelector(".booking-provider-spec");
    const priceEl = document.querySelector(".booking-provider-price");
    const imgEl   = document.querySelector(".booking-provider-img img");
    const idEl    = document.getElementById("bookingDoctorId");

    if (nameEl)  nameEl.textContent  = doctorName;
    if (specEl)  specEl.textContent  = doctorSpec;
    if (priceEl) priceEl.textContent = "سعر الكشف " + doctorPrice;
    if (imgEl)   imgEl.src           = doctorImg;
    if (idEl)    idEl.value          = doctorId;

    const typeEl = document.getElementById("bookingVisitType");
    if (typeEl) typeEl.value = "doctor";

    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    const dtInput = document.querySelector('[name="datetime"]');
    if (dtInput) dtInput.min = now.toISOString().slice(0, 16);

    const overlay = document.getElementById("bookingOverlay");
    if (overlay) {
        overlay.classList.add("open");
        document.body.style.overflow = "hidden";
    }
};

window.closeBookingModal = function () {
    const overlay = document.getElementById("bookingOverlay");
    if (overlay) {
        overlay.classList.remove("open");
        document.body.style.overflow = "";
    }
    const form = document.getElementById("bookingForm");
    if (form) form.reset();
    resetFileLabel();
};

window.closeBookingIfOutside = function (e) {
    if (e.target === document.getElementById("bookingOverlay")) {
        closeBookingModal();
    }
};


window.submitBooking = function (e) {
    e.preventDefault();

    const form = document.getElementById("bookingForm");
    if (!form) return;

    const fileInput = document.getElementById("nationalId");
    if (fileInput && !fileInput.files.length) {
        showToast("يرجى رفع صورة بطاقة الرقم القومي", "warning");
        return;
    }

    closeBookingModal();
    form.reset();
    resetFileLabel();
    openSuccessModal();
};


function openSuccessModal() {
    const overlay = document.getElementById("successOverlay");
    if (overlay) {
        overlay.classList.add("open");
        document.body.style.overflow = "hidden";
    }
}

window.closeSuccessModal = function () {
    const overlay = document.getElementById("successOverlay");
    if (overlay) {
        overlay.classList.remove("open");
        document.body.style.overflow = "";
    }
};


function initFileUpload() {
    const fileInput = document.getElementById("nationalId");
    const fileLabel = document.getElementById("nationalIdLabel");
    if (!fileInput || !fileLabel) return;

    fileInput.addEventListener("change", function () {
        if (fileInput.files && fileInput.files[0]) {
            fileLabel.innerHTML =
                '<i class="fa-solid fa-check-circle" style="color:#28a745;margin-left:6px;"></i>' +
                fileInput.files[0].name;
        } else {
            resetFileLabel();
        }
    });
}

function resetFileLabel() {
    const label = document.getElementById("nationalIdLabel");
    if (label) label.innerHTML = '<i class="fa-solid fa-id-card"></i> رفع صورة بطاقة الرقم القومي';
}

document.addEventListener("keydown", function (e) {
    if (e.key === "Escape") {
        closeBookingModal();
        closeSuccessModal();
    }
});

document.addEventListener("DOMContentLoaded", function () {
    initStars();
    initFileUpload();

    const bookBtn = document.querySelector(".btn.primary");
    if (bookBtn) {
        bookBtn.removeAttribute("onclick");
        bookBtn.addEventListener("click", function () {
            openBookingModal();
        });
    }
});