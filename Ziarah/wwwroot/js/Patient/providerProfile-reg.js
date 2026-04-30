"use strict";

var selectedRating = 0;

function initStars() {
    var stars = document.querySelectorAll(".stars-row i");
    if (!stars.length) return;

    stars.forEach(function (star) {
        star.addEventListener("mouseenter", function () {
            var val = parseInt(this.dataset.value);
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
    var review = document.getElementById("review");
    var comment = review ? review.value.trim() : "";
    if (!comment) {
        showToast("يرجى كتابة تعليقك قبل النشر", "warning");
        return;
    }
    resetRatingForm();
    showToast("تم نشر تقييمك بنجاح ✓");
};

function resetRatingForm() {
    selectedRating = 0;
    document.querySelectorAll(".stars-row i").forEach(function (s) {
        s.classList.remove("selected");
    });
    var review = document.getElementById("review");
    if (review) review.value = "";
}

function showToast(msg, type) {
    document.querySelectorAll(".dp-toast").forEach(function (t) { t.remove(); });
    var colors = { success: "#128CCF", warning: "#e6a817", error: "#e53838" };
    var toast  = document.createElement("div");
    toast.className = "dp-toast";
    toast.textContent = msg;
    toast.style.cssText = [
        "position:fixed", "bottom:30px", "left:50%",
        "transform:translateX(-50%) translateY(20px)",
        "background:" + (colors[type] || colors.success),
        "color:white", "padding:12px 28px", "border-radius:30px",
        "font-size:14px", "font-weight:700", "font-family:'Cairo',sans-serif",
        "box-shadow:0 8px 24px rgba(0,0,0,0.18)", "z-index:99999",
        "opacity:0", "transition:all 0.3s ease", "white-space:nowrap", "pointer-events:none"
    ].join(";");
    document.body.appendChild(toast);
    requestAnimationFrame(function () {
        toast.style.opacity = "1";
        toast.style.transform = "translateX(-50%) translateY(0)";
    });
    setTimeout(function () {
        toast.style.opacity = "0";
        toast.style.transform = "translateX(-50%) translateY(20px)";
        setTimeout(function () { toast.remove(); }, 300);
    }, 3000);
}


var _bookingSource = "account";

window.switchBookingSource = function (mode) {
    _bookingSource = mode;
    var btnAccount = document.getElementById("btnUseAccount");
    var btnNew     = document.getElementById("btnUseNew");
    var preview    = document.getElementById("bookingAccountPreview");
    var newFields  = document.getElementById("bookingNewFields");
    var hidden     = document.getElementById("bookingUseAccount");

    if (mode === "account") {
        if (btnAccount) btnAccount.classList.add("active");
        if (btnNew)     btnNew.classList.remove("active");
        if (preview)    preview.style.display  = "";
        if (newFields)  newFields.style.display = "none";
        if (hidden)     hidden.value            = "true";
        clearFormErrors(document.getElementById("bookingForm"));
    } else {
        if (btnNew)     btnNew.classList.add("active");
        if (btnAccount) btnAccount.classList.remove("active");
        if (preview)    preview.style.display  = "none";
        if (newFields)  newFields.style.display = "";
        if (hidden)     hidden.value            = "false";
    }
};


function getAccountData() {
    if (window.CURRENT_USER) return window.CURRENT_USER;
    return {
        FullName : "محمد أحمد",
        Phone    : "01012345678",
        Address  : "بني سويف - شارع الجمهورية",
        Email    : "user@gmail.com"
    };
}



window.openBookingModal = function () {
    var nameEl  = document.querySelector(".provider-info h2");
    var specEl  = document.querySelector(".specialty");
    var priceEl = document.querySelector(".price-box .price");
    var imgEl   = document.querySelector(".provider-image img");

    var doctorName  = nameEl  ? nameEl.textContent.trim()  : "";
    var doctorSpec  = specEl  ? specEl.textContent.trim()  : "";
    var doctorPrice = priceEl ? priceEl.textContent.trim() : "";
    var doctorImg   = imgEl   ? imgEl.src                  : "";
    var doctorId    = document.body.dataset.doctorId       || "";

    var setTxt = function (id, val) {
        var el = document.getElementById(id);
        if (el) el.textContent = val;
    };
    var elImg = document.getElementById("bookingProviderImg");
    if (elImg) elImg.src = doctorImg;
    setTxt("bookingProviderName",  doctorName);
    setTxt("bookingProviderSpec",  doctorSpec);
    setTxt("bookingProviderPrice", "سعر الكشف " + doctorPrice);
    setTxt("bookingModalTitle",    "طلب زيارة");

    var elId   = document.getElementById("bookingProviderId");
    var elType = document.getElementById("bookingVisitType");
    if (elId)   elId.value   = doctorId;
    if (elType) elType.value = "doctor";

    var user = getAccountData();
    var setVal = function (id, val) {
        var el = document.getElementById(id);
        if (el) el.textContent = val || "—";
    };
    setVal("previewName",    user.FullName);
    setVal("previewPhone",   user.Phone);
    setVal("previewAddress", user.Address);
    setVal("previewEmail",   user.Email || "غير مضاف");

    var now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    var dtInput = document.getElementById("fieldDatetime");
    if (dtInput) dtInput.min = now.toISOString().slice(0, 16);

    switchBookingSource("account");
    var form = document.getElementById("bookingForm");
    if (form) { form.reset(); clearFormErrors(form); }
    resetFileLabel();

    var overlay = document.getElementById("bookingOverlay");
    if (overlay) { overlay.classList.add("open"); document.body.style.overflow = "hidden"; }
};

window.closeBookingModal = function () {
    var overlay = document.getElementById("bookingOverlay");
    if (overlay) { overlay.classList.remove("open"); document.body.style.overflow = ""; }
    var form = document.getElementById("bookingForm");
    if (form) form.reset();
    resetFileLabel();
};




function initBookingModal() {
    var overlay = document.getElementById("bookingOverlay");
    if (overlay) {
        overlay.addEventListener("click", function (e) {
            if (e.target === overlay) closeBookingModal();
        });
    }

    var form = document.getElementById("bookingForm");
    if (form) {
        form.removeAttribute("onsubmit");
        form.addEventListener("submit", handleBookingSubmit);
    }
}



async function handleBookingSubmit(e) {
    e.preventDefault();
    var form      = document.getElementById("bookingForm");
    var submitBtn = form ? form.querySelector(".submit-btn") : null;
    if (!form || !submitBtn) return;
    if (!validateBookingForm(form)) return;

    var originalHTML   = submitBtn.innerHTML;
    submitBtn.disabled = true;
    submitBtn.innerHTML = '<i class="fa-solid fa-spinner fa-spin"></i> جاري الإرسال...';

    try {
        await new Promise(function (r) { setTimeout(r, 800); });
        closeBookingModal();
        form.reset();
        resetFileLabel();
        openSuccessModal();
    } catch (err) {
        showFormError("حدث خطأ أثناء الإرسال، يرجى المحاولة لاحقاً");
        console.error(err);
    } finally {
        submitBtn.disabled  = false;
        submitBtn.innerHTML = originalHTML;
    }
}



function validateBookingForm(form) {
    clearFormErrors(form);
    var valid = true;

    if (_bookingSource === "new") {
        [
            { name: "fullName", label: "الاسم بالكامل", minLength: 3 },
            { name: "phone",    label: "رقم الهاتف", pattern: /^01[0-9]{9}$/, patternMsg: "رقم الهاتف غير صحيح (11 رقم يبدأ بـ 01)" },
            { name: "address",  label: "العنوان", minLength: 10 }
        ].forEach(function (rule) {
            var input = form.elements[rule.name];
            if (!input) return;
            var val = input.value.trim();
            if (!val) { showFieldError(input, rule.label + " مطلوب"); valid = false; return; }
            if (rule.minLength && val.length < rule.minLength) {
                showFieldError(input, rule.label + " يجب أن يكون " + rule.minLength + " أحرف على الأقل");
                valid = false; return;
            }
            if (rule.pattern && !rule.pattern.test(val)) {
                showFieldError(input, rule.patternMsg); valid = false;
            }
        });
        var emailInput = form.elements["email"];
        if (emailInput && emailInput.value.trim() &&
            !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(emailInput.value.trim())) {
            showFieldError(emailInput, "البريد الإلكتروني غير صحيح"); valid = false;
        }
    }

    var dt = form.elements["datetime"];
    if (!dt || !dt.value) {
        showFieldError(dt, "يرجى اختيار الموعد"); valid = false;
    } else if (new Date(dt.value) <= new Date()) {
        showFieldError(dt, "يجب اختيار موعد في المستقبل"); valid = false;
    }

    var cond = form.elements["condition"];
    if (!cond || !cond.value.trim()) {
        showFieldError(cond, "يرجى ذكر الحالة المرضية"); valid = false;
    }

    return valid;
}

function showFieldError(input, message) {
    if (!input) return;
    input.classList.add("input-error");
    var err       = document.createElement("span");
    err.className   = "field-error-msg";
    err.textContent = message;
    input.parentNode.appendChild(err);
}

function clearFormErrors(form) {
    if (!form) return;
    form.querySelectorAll(".input-error").forEach(function (el) { el.classList.remove("input-error"); });
    form.querySelectorAll(".field-error-msg").forEach(function (el) { el.remove(); });
    var errBox = document.getElementById("bookingFormError");
    if (errBox) errBox.remove();
}

function showFormError(message) {
    var errBox = document.getElementById("bookingFormError");
    if (!errBox) {
        errBox = document.createElement("div");
        errBox.id = "bookingFormError";
        errBox.style.cssText = "background:#fde8e8;color:#e53838;padding:10px 16px;border-radius:8px;font-size:13px;font-weight:600;margin-bottom:12px;";
        var form = document.getElementById("bookingForm");
        if (form) form.insertBefore(errBox, form.firstChild);
    }
    errBox.textContent = message;
}




function openSuccessModal() {
    var overlay = document.getElementById("successOverlay");
    if (overlay) { overlay.classList.add("open"); document.body.style.overflow = "hidden"; }
}

window.closeSuccessModal = function () {
    var overlay = document.getElementById("successOverlay");
    if (overlay) { overlay.classList.remove("open"); document.body.style.overflow = ""; }
};



function initFileUpload() {
    var fileInput = document.getElementById("nationalId");
    var fileLabel = document.getElementById("nationalIdLabel");
    if (!fileInput || !fileLabel) return;

    fileInput.addEventListener("change", function () {
        if (fileInput.files && fileInput.files[0]) {
            var file = fileInput.files[0];
            if (file.size > 5 * 1024 * 1024) {
                alert("حجم الملف كبير جداً. الحد الأقصى 5 ميجابايت.");
                fileInput.value = "";
                resetFileLabel();
                return;
            }
            fileLabel.innerHTML =
                '<i class="fa-solid fa-circle-check" style="color:#28a745;"></i> ' + file.name;
        } else {
            resetFileLabel();
        }
    });
}

function resetFileLabel() {
    var lbl = document.getElementById("nationalIdLabel");
    if (lbl) lbl.innerHTML = '<i class="fa-solid fa-upload"></i> رفع صورة بطاقة الرقم القومي';
    var inp = document.getElementById("nationalId");
    if (inp) inp.value = "";
}


function getAntiForgeryToken() {
    var el = document.querySelector('input[name="__RequestVerificationToken"]');
    return el ? el.value : "";
}



document.addEventListener("DOMContentLoaded", function () {
    initStars();
    initFileUpload();
    initBookingModal();

    var bookBtn = document.querySelector(".btn.primary");
    if (bookBtn) {
        bookBtn.removeAttribute("onclick");
        bookBtn.addEventListener("click", function () {
            openBookingModal();
        });
    }
});

document.addEventListener("keydown", function (e) {
    if (e.key !== "Escape") return;
    closeBookingModal();
    closeSuccessModal();
});