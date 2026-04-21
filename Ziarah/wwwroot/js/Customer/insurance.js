"use strict";

let currentType   = "all";
let currentSearch = "";
let currentPage   = 1;
const PAGE_SIZE   = 6;

document.addEventListener("DOMContentLoaded", function () {
    updateDisplay();
});

window.filterCards = function (type, btn) {
    currentType = type;
    currentPage = 1;

    document.querySelectorAll(".filter-tab").forEach(t => t.classList.remove("active"));
    if (btn) btn.classList.add("active");

    updateDisplay();
};

window.searchCards = function (query) {
    currentSearch = query.trim().toLowerCase();
    currentPage   = 1;
    updateDisplay();
};

function updateDisplay() {
    const allCards = Array.from(document.querySelectorAll(".ins-card"));

    const filtered = allCards.filter(card => {
        const typeMatch   = currentType === "all" || card.dataset.type === currentType;
        const name        = card.querySelector(".card-name")?.textContent.toLowerCase() || "";
        const address     = card.querySelector(".card-address span")?.textContent.toLowerCase() || "";
        const searchMatch = !currentSearch || name.includes(currentSearch) || address.includes(currentSearch);
        return typeMatch && searchMatch;
    });

    allCards.forEach(c => c.style.display = "none");

    const totalPages = Math.max(1, Math.ceil(filtered.length / PAGE_SIZE));
    if (currentPage > totalPages) currentPage = 1;

    const start = (currentPage - 1) * PAGE_SIZE;
    const pageItems = filtered.slice(start, start + PAGE_SIZE);

    pageItems.forEach(c => c.style.display = "flex");

    const visibleEl = document.getElementById("visibleCount");
    const totalEl   = document.getElementById("totalCount");
    if (visibleEl) visibleEl.textContent = pageItems.length;
    if (totalEl)   totalEl.textContent   = filtered.length;

    const emptyState = document.getElementById("emptyState");
    if (emptyState) {
        emptyState.classList.toggle("show", filtered.length === 0);
    }

    renderPagination(totalPages);
}

function renderPagination(totalPages) {
    const wrap = document.getElementById("paginationWrap");
    if (!wrap) return;

    wrap.innerHTML = "";

    if (totalPages <= 1) return;

    const prev = createPageBtn("", "arrow" + (currentPage === 1 ? " disabled" : ""));
    prev.innerHTML = '<i class="fa-solid fa-chevron-right"></i>';
    if (currentPage > 1) {
        prev.onclick = () => goToPage(currentPage - 1);
    }
    wrap.appendChild(prev);

    const range = getPageRange(currentPage, totalPages);
    range.forEach(p => {
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
    if (currentPage < totalPages) {
        next.onclick = () => goToPage(currentPage + 1);
    }
    wrap.appendChild(next);
}

function createPageBtn(text, extraClass = "") {
    const btn = document.createElement("button");
    btn.className = "page-btn " + extraClass;
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
    window.scrollTo({ top: document.querySelector(".insurance-main").offsetTop - 80, behavior: "smooth" });
}

window.openMapModal = function () {
    const modal = document.getElementById("mapModal");
    if (modal) {
        modal.classList.add("open");
        document.body.style.overflow = "hidden";
    }
};

window.closeMapModal = function () {
    const modal = document.getElementById("mapModal");
    if (modal) {
        modal.classList.remove("open");
        document.body.style.overflow = "";
    }
};

window.closeMapIfOutside = function (e) {
    if (e.target === document.getElementById("mapModal")) {
        closeMapModal();
    }
};

window.filterMap = function (type, btn) {
    document.querySelectorAll(".map-filter-btn").forEach(b => b.classList.remove("active"));
    if (btn) btn.classList.add("active");

};

document.addEventListener("keydown", function (e) {
    if (e.key === "Escape") closeMapModal();
});