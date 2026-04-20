"use strict";

let currentType   = "doctor"; 
let currentPage   = 1;
const PAGE_SIZE   = 8;


document.addEventListener("DOMContentLoaded", function () {
    bindFilters();
    updateDisplay();
    updateBookBtn();
    initFileUpload();
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
    const nameVal   = (document.getElementById("filterName")?.value    || "").trim().toLowerCase();
    const addrVal   = (document.getElementById("filterAddress")?.value || "").trim().toLowerCase();
    const specVal   = (document.getElementById("filterSpec")?.value    || "").trim();
    const ratingVal = parseFloat(document.getElementById("filterRating")?.value || "0") || 0;
    const priceMax  = parseInt(document.getElementById("filterPrice")?.value  || "99999") || 99999;

    const filtered = allCards.filter(card => {
        const cardType   = card.dataset.type   || "";
        const cardName   = (card.dataset.name  || "").toLowerCase();
        const cardSpec   = (card.dataset.spec  || "");
        const cardPrice  = parseInt(card.dataset.price  || "0") || 0;
        const cardRating = parseFloat(card.dataset.rating || "0") || 0;

        if (cardType !== currentType) return false;
        if (nameVal   && !cardName.includes(nameVal)) return false;
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
    if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1);
    const range = [];
    if (current <= 4) {
        for (let i = 1; i <= 5; i++) range.push(i);
        range.push("...", total);
    } else if (current >= total - 3) {
        range.push(1, "...");
        for (let i = total - 4; i <= total; i++) range.push(i);
    } else {
        range.push(1, "...", current - 1, current, current + 1, "...", total);
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

        const docImg   = document.querySelector(".booking-provider-img img");
        const docName  = document.querySelector(".booking-provider-name");
        const docSpec  = document.querySelector(".booking-provider-spec");
        const docPrice = document.querySelector(".booking-provider-price");
        const hiddenId = document.getElementById("bookingDoctorId");
        const hiddenType = document.getElementById("bookingVisitType");

        if (docImg)   docImg.src          = img;
        if (docName)  docName.textContent  = name;
        if (docSpec)  docSpec.textContent  = spec;
        if (docPrice) docPrice.textContent = "سعر الكشف " + price + " جنيه";
        if (hiddenId) hiddenId.value       = id;
        if (hiddenType) hiddenType.value   = currentType;

        const submitBtn = document.querySelector("#bookingForm .submit-btn");
        if (submitBtn) submitBtn.textContent = currentType === "nurse" ? "طلب ممرض" : "طلب طبيب";

        const modalTitle = document.querySelector(".booking-modal-title");
        if (modalTitle) modalTitle.textContent = currentType === "nurse" ? "طلب ممرض" : "طلب زيارة";
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


window.submitBooking = function (e) {
    e.preventDefault();

    const form = document.getElementById("bookingForm");
    if (!form) return;

    const formData = new FormData(form);

    closeBookingModal();
    form.reset();
    updatePriceLabel();
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
                '<i class="fa-solid fa-check-circle" style="color:#28a745;"></i> ' +
                fileInput.files[0].name;
        } else {
            fileLabel.innerHTML = '<i class="fa-solid fa-upload"></i> رفع صورة بطاقة الرقم القومي';
        }
    });
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