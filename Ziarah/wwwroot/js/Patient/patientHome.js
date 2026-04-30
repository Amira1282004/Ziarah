"use strict";

document.addEventListener("DOMContentLoaded", function () {
    initUserMenuClose();
    initEmergencyModal();
    initBookingModal();
    initFileUpload();
    initNotifBadge();
    handleSpecFromUrl();   // ← فلتر التخصص من الرئيسية
});



// ══════════════════════════════════════════════════════════════════════════════
//  VISIT HISTORY
// ══════════════════════════════════════════════════════════════════════════════
var PAGE_SECTIONS = [".welcome-section", ".symptoms-section", ".providers-section", ".insurance-section"];

window.showVisitHistory = function () {
    PAGE_SECTIONS.forEach(function (sel) {
        document.querySelectorAll(sel).forEach(function (el) { el.style.display = "none"; });
    });
    var sec = document.getElementById("visitHistorySection");
    if (sec) sec.style.display = "";
    filterVisits("all", document.querySelector(".vh-filter-btn"));
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
    if (statusEl) { statusEl.className = "vh-card-status vh-status-cancelled"; statusEl.textContent = "ملغاة"; }
    var actionsEl = card.querySelector(".vh-card-actions");
    if (actionsEl) actionsEl.innerHTML = '<button class="vh-action-btn" onclick="rebookVisit(this)"><i class="fa-solid fa-rotate-right"></i> إعادة الحجز</button>';
};

window.rebookVisit = function () {
    hideVisitHistory();
    setTimeout(function () {
        var sec = document.querySelector(".providers-section");
        if (sec) sec.scrollIntoView({ behavior: "smooth" });
    }, 300);
};

// ══════════════════════════════════════════════════════════════════════════════
//  EMERGENCY MODAL
// ══════════════════════════════════════════════════════════════════════════════
function initEmergencyModal() {
    var overlay = document.getElementById("emergencyModal");
    if (overlay)
        overlay.addEventListener("click", function (e) { if (e.target === overlay) closeEmergency(); });
}

window.openEmergency = function () {
    var modal = document.getElementById("emergencyModal");
    if (modal) { modal.style.display = "flex"; document.body.style.overflow = "hidden"; }
};

window.closeEmergency = function () {
    var modal = document.getElementById("emergencyModal");
    if (modal) { modal.style.display = "none"; document.body.style.overflow = ""; }
};

// ══════════════════════════════════════════════════════════════════════════════
//  BOOKING MODAL
// ══════════════════════════════════════════════════════════════════════════════
var _bookingSource = "account";

function hasSavedNid() {
    return !!(
        document.querySelector('input[name="SavedNidFront"]')?.value &&
        document.querySelector('input[name="SavedNidBack"]')?.value
    );
}

window.switchBookingSource = function (mode) {
    _bookingSource = mode;
    var btnAccount  = document.getElementById("btnUseAccount");
    var btnNew      = document.getElementById("btnUseNew");
    var preview     = document.getElementById("bookingAccountPreview");
    var newFields   = document.getElementById("bookingNewFields");
    var hidden      = document.getElementById("bookingUseAccount");
    var nidFields   = document.getElementById("nidUploadFields");

    if (mode === "account") {
        if (btnAccount) btnAccount.classList.add("active");
        if (btnNew)     btnNew.classList.remove("active");
        if (preview)    preview.style.display  = "";
        if (newFields)  newFields.style.display = "none";
        if (hidden)     hidden.value            = "true";
        if (nidFields)  nidFields.style.display = hasSavedNid() ? "none" : "";
        setNidRequired(!hasSavedNid());
    } else {
        if (btnNew)     btnNew.classList.add("active");
        if (btnAccount) btnAccount.classList.remove("active");
        if (preview)    preview.style.display  = "none";
        if (newFields)  newFields.style.display = "";
        if (hidden)     hidden.value            = "false";
        if (nidFields)  nidFields.style.display = "";
        setNidRequired(true);
    }
};

function setNidRequired(required) {
    var front = document.getElementById("nationalIdFront");
    var back  = document.getElementById("nationalIdBack");
    if (front) { if (required) front.setAttribute("required",""); else front.removeAttribute("required"); }
    if (back)  { if (required) back.setAttribute("required","");  else back.removeAttribute("required");  }
}

function getAccountData() {
    if (window.CURRENT_USER) return window.CURRENT_USER;
    return { FullName: "—", Phone: "—", Email: "—", Address: "" };
}

window.openBookingModal = function (btn) {
    var card = btn ? btn.closest(".provider-card") : null;
    if (card) {
        var id       = card.dataset.id    || "";
        var name     = card.dataset.name  || "";
        var spec     = card.dataset.spec  || "";
        var price    = card.dataset.price || "";
        var img      = card.dataset.img   || "";
        var hasPhoto = (card.dataset.hasPhoto || "false") === "true";
        var cardType = card.dataset.type  || "doctor";

        var imgWrap  = document.querySelector(".booking-provider-img");
        if (imgWrap) {
            if (hasPhoto && img) {
                imgWrap.innerHTML = '<img src="' + img + '" alt="' + name + '">';
            } else {
                var ic = cardType === "nurse" ? "fa-user-nurse" : "fa-user-doctor";
                imgWrap.innerHTML = '<div class="provider-avatar-fallback ' + cardType + '"><i class="fa-solid ' + ic + '"></i></div>';
            }
        }
        var setTxt = function (sel, val) { var el = document.querySelector(sel); if (el) el.textContent = val; };
        setTxt(".booking-provider-name",  name);
        setTxt(".booking-provider-spec",  spec);
        setTxt(".booking-provider-price", parseFloat(price||"0") > 0 ? "سعر الكشف " + parseFloat(price) + " جنيه" : "سعر الكشف غير محدد");

        var hidId   = document.getElementById("bookingProviderId");
        var hidType = document.getElementById("bookingVisitType");
        if (hidId)   hidId.value   = id;
        if (hidType) hidType.value = cardType;

        var submitBtn = document.querySelector("#bookingForm .submit-btn");
        if (submitBtn) submitBtn.textContent = cardType === "nurse" ? "طلب ممرض" : "طلب طبيب";

        var modalTitle = document.querySelector(".booking-modal-title");
        if (modalTitle) modalTitle.textContent = cardType === "nurse" ? "طلب ممرض" : "طلب زيارة";
    }

    var user = getAccountData();
    var setVal = function (id, val) { var el = document.getElementById(id); if (el) el.textContent = val || "—"; };
    setVal("previewName",  user.FullName);
    setVal("previewPhone", user.Phone);
    setVal("previewEmail", user.Email);

    var previewAddr = document.getElementById("previewAddress");
    if (previewAddr) previewAddr.textContent = user.Address || "—";

    var dtInput = document.getElementById("bookingRequestedVisitAt");
    if (dtInput) {
        var now = new Date();
        now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
        dtInput.min = now.toISOString().slice(0, 16);
    }

    switchBookingSource("account");

    var form = document.getElementById("bookingForm");
    if (form) form.reset();
    resetNidLabels();

    var overlay = document.getElementById("bookingOverlay");
    if (overlay) { overlay.classList.add("open"); document.body.style.overflow = "hidden"; }
};

window.closeBookingModal = function () {
    var overlay = document.getElementById("bookingOverlay");
    if (overlay) { overlay.classList.remove("open"); document.body.style.overflow = ""; }
};

window.closeBookingIfOutside = function (e) {
    if (e.target === document.getElementById("bookingOverlay")) closeBookingModal();
};

function initBookingModal() {
    var overlay = document.getElementById("bookingOverlay");
    if (overlay)
        overlay.addEventListener("click", function (e) { if (e.target === overlay) closeBookingModal(); });
    var form = document.getElementById("bookingForm");
    if (form) form.addEventListener("submit", handleBookingSubmit);
}

async function handleBookingSubmit(e) {
    e.preventDefault();
    var form      = document.getElementById("bookingForm");
    var submitBtn = document.getElementById("bookingSubmitBtn") || form?.querySelector(".submit-btn");
    if (!form || !submitBtn) return;

    var providerId = parseInt(document.getElementById("bookingProviderId")?.value || "0", 10);
    if (!providerId) { showBookingToast("يرجى اختيار مقدم خدمة أولاً.", true); return; }

    if (!hasSavedNid() || _bookingSource === "new") {
        var front = document.getElementById("nationalIdFront");
        var back  = document.getElementById("nationalIdBack");
        if (!front?.files?.length || !back?.files?.length) {
            showBookingToast("يرجى رفع صورتي بطاقة الرقم القومي (الوجه والظهر).", true);
            return;
        }
    }

    var originalHTML = submitBtn.innerHTML;
    submitBtn.disabled = true;
    submitBtn.innerHTML = '<i class="fa-solid fa-spinner fa-spin"></i> جاري الإرسال...';

    try {
        var res  = await fetch(form.action, {
            method     : "POST",
            body       : new FormData(form),
            credentials: "same-origin",
            headers    : { Accept: "application/json" }
        });
        var data = {};
        try { data = await res.json(); } catch { data = {}; }

        if (!res.ok || data.success === false) {
            var errs = Array.isArray(data.errors) ? data.errors : [data.message || "تعذر إرسال الطلب."];
            showBookingToast(errs.join(" "), true);
            return;
        }

        closeBookingModal();
        form.reset();
        resetNidLabels();
        openSuccessModal();
    } catch (err) {
        showBookingToast("حدث خطأ في الاتصال. تحقق من الشبكة وحاول مرة أخرى.", true);
        console.error(err);
    } finally {
        submitBtn.disabled  = false;
        submitBtn.innerHTML = originalHTML;
    }
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
    function bind(inputId, labelId, defaultHtml) {
        var input = document.getElementById(inputId);
        var label = document.getElementById(labelId);
        if (!input || !label) return;
        input.addEventListener("change", function () {
            if (input.files && input.files[0]) {
                if (input.files[0].size > 5 * 1024 * 1024) {
                    showBookingToast("حجم الملف كبير جداً. الحد الأقصى 5 ميجابايت.", true);
                    input.value = ""; label.innerHTML = defaultHtml; return;
                }
                label.innerHTML = '<i class="fa-solid fa-circle-check" style="color:#28a745;"></i> ' + input.files[0].name;
            } else {
                label.innerHTML = defaultHtml;
            }
        });
    }
    bind("nationalIdFront", "nationalIdFrontLabel", '<i class="fa-solid fa-upload"></i> صورة وجه بطاقة الرقم القومي');
    bind("nationalIdBack",  "nationalIdBackLabel",  '<i class="fa-solid fa-upload"></i> صورة ظهر بطاقة الرقم القومي');
}

function resetNidLabels() {
    var f = document.getElementById("nationalIdFrontLabel");
    var b = document.getElementById("nationalIdBackLabel");
    if (f) f.innerHTML = '<i class="fa-solid fa-upload"></i> صورة وجه بطاقة الرقم القومي';
    if (b) b.innerHTML = '<i class="fa-solid fa-upload"></i> صورة ظهر بطاقة الرقم القومي';
}

function showBookingToast(msg, isError) {
    document.querySelectorAll(".booking-toast").forEach(function (t) { t.remove(); });
    var toast = document.createElement("div");
    toast.className = "booking-toast";
    toast.textContent = msg;
    toast.style.cssText =
        "position:fixed;bottom:28px;left:50%;transform:translateX(-50%);z-index:100000;" +
        "max-width:92%;padding:12px 22px;border-radius:12px;font:600 14px Cairo,sans-serif;" +
        "box-shadow:0 8px 28px rgba(0,0,0,.2);color:#fff;background:" +
        (isError ? "#c0392b" : "#128CCF") + ";text-align:center;";
    document.body.appendChild(toast);
    setTimeout(function () { toast.remove(); }, 5000);
}

function handleSpecFromUrl() {
    // هذه الدالة تُستدعى في الصفحة الرئيسية فقط — لا يوجد شيء لفعله هنا
    // الفلترة تحدث في requestVisit.js عند تحميل الصفحة
}


document.addEventListener("keydown", function (e) {
    if (e.key !== "Escape") return;
    closeBookingModal();
    closeSuccessModal();
    closeEmergency();
    closeNotifMenu();
});