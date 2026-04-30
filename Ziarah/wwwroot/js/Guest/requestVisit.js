"use strict";

let currentType   = "doctor"; 
let currentPage   = 1;
const PAGE_SIZE   = 8;


document.addEventListener("DOMContentLoaded", function () {
    bindFilters();
    updateDisplay();
    updateBookBtn();
    bindNationalIdFileLabels();
    const bookingForm = document.getElementById("bookingForm");
    if (bookingForm) {
        bookingForm.addEventListener("submit", submitBooking);
    }
});


document.querySelectorAll(".visit-type-btn").forEach(function (btn, idx) {
    btn.addEventListener("click", function () {
        document.querySelectorAll(".visit-type-btn").forEach(b => b.classList.remove("active"));
        btn.classList.add("active");

        currentType = idx === 0 ? "doctor" : "nurse";
        currentPage = 1;

        const specFilter = document.getElementById("specFilter");
        if (specFilter) specFilter.style.display = currentType === "nurse" ? "none" : "";

        updateDisplay();
        updateBookBtn();
    });
});

function bindFilters() {
    const nameInput    = document.getElementById("filterName");
    const addressInput = document.getElementById("filterAddress");
    const specSelect   = document.getElementById("filterSpec");
    const ratingSelect = document.getElementById("filterRating");
    const resetBtn     = document.querySelector(".reset-btn");

    if (nameInput)    nameInput.addEventListener("input",  applyFilters);
    if (addressInput) addressInput.addEventListener("input", applyFilters);
    if (specSelect)   specSelect.addEventListener("change", applyFilters);
    if (ratingSelect) ratingSelect.addEventListener("change", applyFilters);

    if (resetBtn) resetBtn.addEventListener("click", resetFilters);
}

function applyFilters() {
    currentPage = 1;
    updateDisplay();
}

function resetFilters() {
    const nameInput    = document.getElementById("filterName");
    const addressInput = document.getElementById("filterAddress");
    const specSelect   = document.getElementById("filterSpec");
    const ratingSelect = document.getElementById("filterRating");
    const priceRange   = document.getElementById("filterPrice");

    if (nameInput)    nameInput.value    = "";
    if (addressInput) addressInput.value = "";
    if (specSelect)   specSelect.value   = "";
    if (ratingSelect) ratingSelect.value = "";
    if (priceRange) {
        priceRange.value = priceRange.max;
        updatePriceLabel();
    }

    currentPage = 1;
    updateDisplay();
}

window.updatePriceLabel = function () {
    const priceRange = document.getElementById("filterPrice");
    const priceLabel = document.getElementById("priceLabel");
    if (priceRange && priceLabel) {
        priceLabel.textContent = "حتى " + priceRange.value + " جنيه";
    }
};


function updateDisplay() {
    const allCards  = Array.from(document.querySelectorAll("#cardsGrid .provider-card"));
    const nameVal   = normalizeArabic((document.getElementById("filterName")?.value    || "").trim());
    const addrVal   = normalizeArabic((document.getElementById("filterAddress")?.value || "").trim());
    const specVal   = normalizeArabic((document.getElementById("filterSpec")?.value    || "").trim());
    const ratingVal = parseFloat(document.getElementById("filterRating")?.value || "0") || 0;
    const priceMax  = parseInt(document.getElementById("filterPrice")?.value  || "99999") || 99999;

    const filtered = allCards.filter(card => {
        const cardType   = card.dataset.type   || "";
        const cardName   = normalizeArabic(card.dataset.name  || "");
        const cardSpec   = normalizeArabic(card.dataset.spec  || "");
        const cardAddr   = normalizeArabic(card.dataset.address || "");
        const cardPrice  = parseInt(card.dataset.price  || "0") || 0;
        const cardRating = parseFloat(card.dataset.rating || "0") || 0;

        if (cardType !== currentType) return false;
        if (nameVal   && !cardName.includes(nameVal)) return false;
        if (addrVal   && !cardAddr.includes(addrVal)) return false;
        if (specVal   && cardSpec !== specVal) return false;
        if (ratingVal && cardRating < ratingVal) return false;
        if (cardPrice > priceMax) return false;

        return true;
    });

    allCards.forEach(c => { c.style.display = "none"; });

    const totalPages = Math.max(1, Math.ceil(filtered.length / PAGE_SIZE));
    if (currentPage > totalPages) currentPage = 1;

    const start     = (currentPage - 1) * PAGE_SIZE;
    const pageItems = filtered.slice(start, start + PAGE_SIZE);

    pageItems.forEach(c => { c.style.display = "flex"; });

    const visibleEl = document.getElementById("visibleCount");
    const totalEl   = document.getElementById("totalCount");
    if (visibleEl) visibleEl.textContent = pageItems.length;
    if (totalEl)   totalEl.textContent   = filtered.length;

    const emptyState = document.getElementById("emptyState");
    if (emptyState) emptyState.classList.toggle("show", filtered.length === 0);

    renderPagination(totalPages);
}

function normalizeArabic(text) {
    return (text || "")
        .toLowerCase()
        .replaceAll("أ", "ا")
        .replaceAll("إ", "ا")
        .replaceAll("آ", "ا")
        .replaceAll("ى", "ي")
        .replaceAll("ة", "ه")
        .replaceAll("ؤ", "و")
        .replaceAll("ئ", "ي")
        .trim();
}


function renderPagination(totalPages) {
    const wrap = document.getElementById("paginationWrap");
    if (!wrap) return;
    wrap.innerHTML = "";
    if (totalPages <= 1) return;

    const prev = createPageBtn("", "arrow" + (currentPage === 1 ? " disabled" : ""));
    prev.innerHTML = '<i class="fa-solid fa-chevron-right"></i>';
    if (currentPage > 1) prev.onclick = () => goToPage(currentPage - 1);
    wrap.appendChild(prev);

    getPageRange(currentPage, totalPages).forEach(p => {
        if (p === "...") {
            const dots = document.createElement("span");
            dots.textContent = "...";
            dots.style.cssText = "padding:0 6px; color:#888; line-height:38px;";
            wrap.appendChild(dots);
        } else {
            const btn = createPageBtn(p, p === currentPage ? "active" : "");
            btn.onclick = () => goToPage(p);
            wrap.appendChild(btn);
        }
    });

    const next = createPageBtn("", "arrow" + (currentPage === totalPages ? " disabled" : ""));
    next.innerHTML = '<i class="fa-solid fa-chevron-left"></i>';
    if (currentPage < totalPages) next.onclick = () => goToPage(currentPage + 1);
    wrap.appendChild(next);
}

function createPageBtn(text, extraClass) {
    const btn = document.createElement("button");
    btn.className = "page-btn " + (extraClass || "");
    if (text) btn.textContent = text;
    return btn;
}

function getPageRange(current, total) {
    const range = [];
    if (total <= 3) {
        return Array.from({ length: total }, (_, i) => i + 1);
    }

    let start = current;
    if (start > total - 2) {
        start = Math.max(1, total - 2);
    }

    for (let i = start; i <= Math.min(total, start + 2); i++) {
        range.push(i);
    }

    return range;
}

function goToPage(page) {
    currentPage = page;
    updateDisplay();
    const main = document.querySelector(".visit-main");
    if (main) window.scrollTo({ top: main.offsetTop - 80, behavior: "smooth" });
}

window.openBookingModal = function (btn) {
    const card = btn ? btn.closest(".provider-card") : null;

    if (card) {
        const id    = card.dataset.id    || "";
        const name  = card.dataset.name  || "";
        const spec  = card.dataset.spec  || "";
        const price = card.dataset.price || "";
        const img   = card.dataset.img   || "";
        const hasPhoto = (card.dataset.hasPhoto || "false") === "true";
        const cardType = card.dataset.type || currentType;

        const docImgWrap = document.querySelector(".booking-provider-img");
        const docImg   = document.querySelector(".booking-provider-img img");
        const docName  = document.querySelector(".booking-provider-name");
        const docSpec  = document.querySelector(".booking-provider-spec");
        const docPrice = document.querySelector(".booking-provider-price");
        const hiddenId = document.getElementById("bookingProviderId");
        const hiddenType = document.getElementById("bookingVisitType");

        if (docImgWrap) {
            if (hasPhoto && img) {
                docImgWrap.innerHTML = '<img src="' + img + '" alt="' + name + '">';
            } else {
                const iconClass = cardType === "nurse" ? "fa-user-nurse" : "fa-user-doctor";
                const roleClass = cardType === "nurse" ? "nurse" : "doctor";
                docImgWrap.innerHTML = '<div class="provider-avatar-fallback ' + roleClass + '"><i class="fa-solid ' + iconClass + '"></i></div>';
            }
        } else if (docImg) {
            docImg.src = img;
        }
        if (docName)  docName.textContent  = name;
        if (docSpec)  docSpec.textContent  = spec;
        if (docPrice) {
            const numericPrice = parseFloat(price || "0");
            docPrice.textContent = numericPrice > 0 ? ("سعر الكشف " + numericPrice + " جنيه") : "سعر الكشف غير محدد";
        }
        if (hiddenId) hiddenId.value = id;
        if (hiddenType) hiddenType.value = cardType;

        const dtInput = document.getElementById("bookingRequestedVisitAt");
        if (dtInput) {
            const now = new Date();
            now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
            dtInput.min = now.toISOString().slice(0, 16);
        }

        const submitBtn = document.querySelector("#bookingForm .submit-btn");
        if (submitBtn) submitBtn.textContent = cardType === "nurse" ? "طلب ممرض" : "طلب طبيب";

        const modalTitle = document.querySelector(".booking-modal-title");
        if (modalTitle) modalTitle.textContent = cardType === "nurse" ? "طلب ممرض" : "طلب زيارة";
    }

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
        showVisitRequestToast("يرجى اختيار مقدم خدمة من القائمة أولاً.", true);
        return;
    }

    const front = document.getElementById("nationalIdFront");
    const back = document.getElementById("nationalIdBack");
    if (!front?.files?.length || !back?.files?.length) {
        showVisitRequestToast("يرجى رفع صورتي بطاقة الرقم القومي (الوجه والظهر).", true);
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
            showVisitRequestToast(errs.join(" "), true);
            return;
        }

        closeBookingModal();
        form.reset();
        resetNationalIdLabels();
        updatePriceLabel();
        openSuccessModal();
    } catch {
        showVisitRequestToast("حدث خطأ في الاتصال. تحقق من الشبكة وحاول مرة أخرى.", true);
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
                    '<i class="fa-solid fa-check-circle" style="color:#28a745;"></i> ' +
                    fileInput.files[0].name;
            } else {
                fileLabel.innerHTML = defaultHtml;
            }
        });
    }
    bind("nationalIdFront", "nationalIdFrontLabel", '<i class="fa-solid fa-upload"></i> صورة وجه بطاقة الرقم القومي');
    bind("nationalIdBack", "nationalIdBackLabel", '<i class="fa-solid fa-upload"></i> صورة ظهر بطاقة الرقم القومي');
}

function resetNationalIdLabels() {
    const f = document.getElementById("nationalIdFrontLabel");
    const b = document.getElementById("nationalIdBackLabel");
    if (f) f.innerHTML = '<i class="fa-solid fa-upload"></i> صورة وجه بطاقة الرقم القومي';
    if (b) b.innerHTML = '<i class="fa-solid fa-upload"></i> صورة ظهر بطاقة الرقم القومي';
}

function showVisitRequestToast(msg, isError) {
    document.querySelectorAll(".visit-req-toast").forEach(function (t) { t.remove(); });
    const toast = document.createElement("div");
    toast.className = "visit-req-toast";
    toast.textContent = msg;
    toast.style.cssText =
        "position:fixed;bottom:28px;left:50%;transform:translateX(-50%);z-index:100000;" +
        "max-width:92%;padding:12px 22px;border-radius:12px;font:600 14px Cairo,sans-serif;" +
        "box-shadow:0 8px 28px rgba(0,0,0,.2);color:#fff;background:" +
        (isError ? "#c0392b" : "#128CCF") + ";text-align:center;";
    document.body.appendChild(toast);
    setTimeout(function () {
        toast.remove();
    }, 5000);
}


function updateBookBtn() {
    const label = currentType === "nurse" ? "طلب ممرض" : "طلب طبيب";
    document.querySelectorAll(".book-btn").forEach(btn => {
        btn.textContent = label;
    });
}

document.addEventListener("keydown", function (e) {
    if (e.key === "Escape") {
        closeBookingModal();
        closeSuccessModal();
    }
});