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
        error: "#e53838"
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
    const doctorName = document.querySelector(".provider-info h2")?.textContent.trim() || "";
    const doctorSpec = document.querySelector(".specialty")?.textContent.trim() || "";
    const doctorPrice = document.querySelector(".price-box .price")?.textContent.trim() || "";
    const doctorImg = document.querySelector(".provider-image img")?.src || "";
    const profileSection = document.querySelector(".provider-section");
    const doctorId = profileSection?.dataset.providerId || "";
    const providerType = profileSection?.dataset.providerType || "doctor";

    const nameEl = document.querySelector(".booking-provider-name");
    const specEl = document.querySelector(".booking-provider-spec");
    const priceEl = document.querySelector(".booking-provider-price");
    const imgEl = document.querySelector(".booking-provider-img img");
    const idEl = document.getElementById("bookingProviderId");

    if (nameEl) nameEl.textContent = doctorName;
    if (specEl) specEl.textContent = doctorSpec;
    if (priceEl) priceEl.textContent = "سعر الكشف " + doctorPrice;
    if (imgEl) imgEl.src = doctorImg;
    if (idEl) idEl.value = doctorId;

    const typeEl = document.getElementById("bookingVisitType");
    if (typeEl) typeEl.value = providerType;

    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    const dtInput = document.getElementById("bookingRequestedVisitAt");
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
    resetNationalIdLabels();
};

window.closeBookingIfOutside = function (e) {
    if (e.target === document.getElementById("bookingOverlay")) {
        closeBookingModal();
    }
};


async function submitBooking(e) {
    e.preventDefault();

    const form = document.getElementById("bookingForm");
    if (!form) return;

    const providerId = parseInt(document.getElementById("bookingProviderId")?.value || "0", 10);
    if (!providerId) {
        showToast("تعذر تحديد مقدم الخدمة. أعد فتح الصفحة.", "warning");
        return;
    }

    const front = document.getElementById("nationalIdFront");
    const back = document.getElementById("nationalIdBack");
    if (!front?.files?.length || !back?.files?.length) {
        showToast("يرجى رفع صورتي بطاقة الرقم القومي (الوجه والظهر).", "warning");
        return;
    }

    const submitBtn = document.getElementById("bookingSubmitBtn") || form.querySelector(".submit-btn");
    if (submitBtn) submitBtn.disabled = true;

    try {
        const res = await fetch(form.action, {
            method: "POST",
            body: new FormData(form),
            credentials: "same-origin",
            headers: { Accept: "application/json" }
        });

        let data = {};
        try {
            data = await res.json();
        } catch {
            data = {};
        }

        if (!res.ok || data.success === false) {
            const errs = Array.isArray(data.errors) ? data.errors : [data.message || "تعذر إرسال الطلب. حاول مرة أخرى."];
            showToast(errs.join(" "), "error");
            return;
        }

        closeBookingModal();
        form.reset();
        resetNationalIdLabels();
        openSuccessModal();
    } catch {
        showToast("حدث خطأ في الاتصال. تحقق من الشبكة وحاول مرة أخرى.", "error");
    } finally {
        if (submitBtn) submitBtn.disabled = false;
    }
}


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


function bindNationalIdFileLabels() {
    function bind(inputId, labelId, defaultHtml) {
        const fileInput = document.getElementById(inputId);
        const fileLabel = document.getElementById(labelId);
        if (!fileInput || !fileLabel) return;
        fileInput.addEventListener("change", function () {
            if (fileInput.files && fileInput.files[0]) {
                fileLabel.innerHTML =
                    '<i class="fa-solid fa-check-circle" style="color:#28a745;margin-left:6px;"></i>' +
                    fileInput.files[0].name;
            } else {
                fileLabel.innerHTML = defaultHtml;
            }
        });
    }
    bind("nationalIdFront", "nationalIdFrontLabel", '<i class="fa-solid fa-id-card"></i> صورة وجه بطاقة الرقم القومي');
    bind("nationalIdBack", "nationalIdBackLabel", '<i class="fa-solid fa-id-card"></i> صورة ظهر بطاقة الرقم القومي');
}

function resetNationalIdLabels() {
    const f = document.getElementById("nationalIdFrontLabel");
    const b = document.getElementById("nationalIdBackLabel");
    if (f) f.innerHTML = '<i class="fa-solid fa-id-card"></i> صورة وجه بطاقة الرقم القومي';
    if (b) b.innerHTML = '<i class="fa-solid fa-id-card"></i> صورة ظهر بطاقة الرقم القومي';
}

document.addEventListener("keydown", function (e) {
    if (e.key === "Escape") {
        closeBookingModal();
        closeSuccessModal();
    }
});

document.addEventListener("DOMContentLoaded", function () {
    initStars();
    bindNationalIdFileLabels();

    const bookingForm = document.getElementById("bookingForm");
    if (bookingForm) {
        bookingForm.addEventListener("submit", submitBooking);
    }

    const bookBtn = document.querySelector(".btn.primary");
    if (bookBtn) {
        bookBtn.removeAttribute("onclick");
        bookBtn.addEventListener("click", function () {
            openBookingModal();
        });
    }
});