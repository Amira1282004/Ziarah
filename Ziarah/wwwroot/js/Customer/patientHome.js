"use strict";

document.addEventListener("DOMContentLoaded", function () {
    initUserMenuClose();
    initEmergencyModal();
    initBookingModal();
    initFileUpload();
    initNotifBadge();
});



function initUserMenuClose() {
    document.addEventListener("click", function (e) {
        var userMenu = document.querySelector(".user-menu");
        var userDD   = document.getElementById("userDropdown");
        if (userDD && userMenu && !userMenu.contains(e.target)) {
            userDD.classList.remove("open");
        }
        var notifMenu = document.getElementById("notifMenu");
        var notifDD   = document.getElementById("notifDropdown");
        if (notifDD && notifMenu && !notifMenu.contains(e.target)) {
            notifDD.classList.remove("open");
        }
    });
}

window.toggleUserMenu = function () {
    var dd = document.getElementById("userDropdown");
    if (dd) dd.classList.toggle("open");
};

function getAccountData() {
    if (window.CURRENT_USER) return window.CURRENT_USER;
    return {
        FullName : (document.getElementById("userName") || {}).textContent || "المستخدم",
        Phone    : "01012345678",
        Address  : "بني سويف - شارع الجمهورية",
        Email    : "user@gmail.com"
    };
}


function initNotifBadge() {
    updateNotifBadge();
}

function updateNotifBadge() {
    var badge = document.getElementById("notifBadge");
    if (!badge) return;
    var count = document.querySelectorAll("#notifList .notif-item.unread").length;
    badge.textContent = count;
    badge.classList.toggle("hidden", count === 0);
}

window.toggleNotifMenu = function () {
    var dd = document.getElementById("notifDropdown");
    if (dd) dd.classList.toggle("open");
};

window.closeNotifMenu = function () {
    var dd = document.getElementById("notifDropdown");
    if (dd) dd.classList.remove("open");
};

window.markRead = function (el) {
    if (!el) return;
    el.classList.remove("unread");
    updateNotifBadge();
};

window.markAllRead = function () {
    document.querySelectorAll("#notifList .notif-item.unread").forEach(function (item) {
        item.classList.remove("unread");
    });
    updateNotifBadge();
};



var PAGE_SECTIONS = [".welcome-section", ".symptoms-section", ".providers-section", ".insurance-section"];

window.showVisitHistory = function () {
    PAGE_SECTIONS.forEach(function (sel) {
        document.querySelectorAll(sel).forEach(function (el) { el.style.display = "none"; });
    });
    var sec = document.getElementById("visitHistorySection");
    if (sec) sec.style.display = "";

    document.querySelectorAll(".vh-filter-btn").forEach(function (b) { b.classList.remove("active"); });
    var allBtn = document.querySelector(".vh-filter-btn");
    if (allBtn) allBtn.classList.add("active");

    filterVisits("all", null);
    window.scrollTo({ top: 0, behavior: "smooth" });
};

window.hideVisitHistory = function () {
    var sec = document.getElementById("visitHistorySection");
    if (sec) sec.style.display = "none";
    PAGE_SECTIONS.forEach(function (sel) {
        document.querySelectorAll(sel).forEach(function (el) { el.style.display = ""; });
    });
    window.scrollTo({ top: 0, behavior: "smooth" });
};

window.filterVisits = function (status, btn) {
    document.querySelectorAll(".vh-filter-btn").forEach(function (b) { b.classList.remove("active"); });
    if (btn) btn.classList.add("active");

    var cards  = document.querySelectorAll("#vhList .vh-card");
    var hasAny = false;
    cards.forEach(function (card) {
        var match = status === "all" || card.dataset.status === status;
        card.style.display = match ? "" : "none";
        if (match) hasAny = true;
    });

    var emptyEl = document.getElementById("vhEmpty");
    if (emptyEl) emptyEl.style.display = hasAny ? "none" : "";
};

window.cancelVisit = function (btn) {
    if (!confirm("هل تريد إلغاء هذه الزيارة؟")) return;
    var card = btn.closest(".vh-card");
    if (!card) return;

    card.dataset.status = "cancelled";
    var statusEl = card.querySelector(".vh-card-status");
    if (statusEl) {
        statusEl.className   = "vh-card-status vh-status-cancelled";
        statusEl.textContent = "ملغاة";
    }
    var actionsEl = card.querySelector(".vh-card-actions");
    if (actionsEl) {
        actionsEl.innerHTML = '<button class="vh-action-btn" onclick="rebookVisit(this)"><i class="fa-solid fa-rotate-right"></i> إعادة الحجز</button>';
    }
};



window.rebookVisit = function () {
    hideVisitHistory();
    setTimeout(function () {
        var sec = document.querySelector(".providers-section");
        if (sec) sec.scrollIntoView({ behavior: "smooth" });
    }, 300);
};


function initEmergencyModal() {
    var overlay = document.getElementById("emergencyModal");
    if (overlay) {
        overlay.addEventListener("click", function (e) { if (e.target === overlay) closeEmergency(); });
    }
}

window.openEmergency = function () {
    var modal = document.getElementById("emergencyModal");
    if (modal) { modal.style.display = "flex"; document.body.style.overflow = "hidden"; }
};

window.closeEmergency = function () {
    var modal = document.getElementById("emergencyModal");
    if (modal) { modal.style.display = "none"; document.body.style.overflow = ""; }
};


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

window.openBookingModal = function (btn) {
    var card = btn ? btn.closest(".provider-card") : null;
    if (card) {
        var type  = card.dataset.type  || "doctor";
        var name  = card.dataset.name  || "";
        var spec  = card.dataset.spec  || "";
        var price = card.dataset.price || "";
        var img   = card.dataset.img   || "";
        var id    = card.dataset.id    || "";

        var setTxt = function (elId, val) { var el = document.getElementById(elId); if (el) el.textContent = val; };
        var elImg  = document.getElementById("bookingProviderImg");
        if (elImg) elImg.src = img;
        setTxt("bookingProviderName",  name);
        setTxt("bookingProviderSpec",  spec);
        setTxt("bookingProviderPrice", "سعر الكشف " + price + " جنيه");
        setTxt("bookingModalTitle",    type === "nurse" ? "طلب ممرض/ة" : "طلب زيارة");

        var elId   = document.getElementById("bookingProviderId");
        var elType = document.getElementById("bookingVisitType");
        if (elId)   elId.value   = id;
        if (elType) elType.value = type;
    }

    var user   = getAccountData();
    var setVal = function (elId, val) { var el = document.getElementById(elId); if (el) el.textContent = val || "—"; };
    setVal("previewName",    user.FullName);
    setVal("previewPhone",   user.Phone);
    setVal("previewAddress", user.Address);
    setVal("previewEmail",   user.Email || "غير مضاف");

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
};

function initBookingModal() {
    var overlay = document.getElementById("bookingOverlay");
    if (overlay) {
        overlay.addEventListener("click", function (e) { if (e.target === overlay) closeBookingModal(); });
    }
    var form = document.getElementById("bookingForm");
    if (form) form.addEventListener("submit", handleBookingSubmit);
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
            if (rule.minLength && val.length < rule.minLength) { showFieldError(input, rule.label + " يجب أن يكون " + rule.minLength + " أحرف على الأقل"); valid = false; return; }
            if (rule.pattern && !rule.pattern.test(val)) { showFieldError(input, rule.patternMsg); valid = false; }
        });
        var emailInput = form.elements["email"];
        if (emailInput && emailInput.value.trim() && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(emailInput.value.trim())) {
            showFieldError(emailInput, "البريد الإلكتروني غير صحيح"); valid = false;
        }
    }

    var dt = form.elements["datetime"];
    if (!dt || !dt.value) { showFieldError(dt, "يرجى اختيار الموعد"); valid = false; }
    else if (new Date(dt.value) <= new Date()) { showFieldError(dt, "يجب اختيار موعد في المستقبل"); valid = false; }

    var cond = form.elements["condition"];
    if (!cond || !cond.value.trim()) { showFieldError(cond, "يرجى ذكر الحالة المرضية"); valid = false; }

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
            fileLabel.innerHTML = '<i class="fa-solid fa-circle-check" style="color:#28a745;"></i> ' + file.name;
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


function openSuccessModal() {
    var overlay = document.getElementById("successOverlay");
    if (overlay) { overlay.classList.add("open"); document.body.style.overflow = "hidden"; }
}

window.closeSuccessModal = function () {
    var overlay = document.getElementById("successOverlay");
    if (overlay) { overlay.classList.remove("open"); document.body.style.overflow = ""; }
};



document.addEventListener("keydown", function (e) {
    if (e.key !== "Escape") return;
    closeBookingModal();
    closeSuccessModal();
    closeEmergency();
    closeNotifMenu();
});


function getAntiForgeryToken() {
    var el = document.querySelector('input[name="__RequestVerificationToken"]');
    return el ? el.value : "";
}