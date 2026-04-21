document.addEventListener("DOMContentLoaded", function () {
    window.closeTopBar = function () {
        document.querySelector(".top-bar").style.display = "none";
    };

    window.toggleMenu = function () {
        const nav = document.querySelector(".nav");
        const hamburger = document.querySelector(".hamburger");
        nav.classList.toggle("active");
        hamburger.classList.toggle("open");
    };

    document.querySelectorAll(".nav a").forEach(link => {
        link.addEventListener("click", () => {
            document.querySelector(".nav").classList.remove("active");
            document.querySelector(".hamburger").classList.remove("open");
        });
    });

    if (sessionStorage.getItem("theme") === "dark") {
        document.body.classList.add("dark-mode");
        const icon = document.querySelector(".theme-toggle i");
        if (icon) icon.classList.replace("fa-moon", "fa-sun");
    }

    window.toggleTheme = function () {
        document.body.classList.toggle("dark-mode");
        const icons = document.querySelectorAll(".theme-toggle i, .nav-theme i");
        icons.forEach(icon => {
            if (document.body.classList.contains("dark-mode")) {
                icon.classList.replace("fa-moon", "fa-sun");
            } else {
                icon.classList.replace("fa-sun", "fa-moon");
            }
        });
        sessionStorage.setItem("theme", document.body.classList.contains("dark-mode") ? "dark" : "light");
    };

    window.changeLang = function (lang) {
        if (lang === 'en') {
            window.location.href = 'https://translate.google.com/translate?sl=ar&tl=en&u=' + encodeURIComponent(window.location.href);
        } else {
            const originalUrl = window.location.href
                .replace(/https:\/\/translate\.google\.com\/translate\?.*&u=/, '')
                .replace(/https:\/\/[^.]+\.translate\.goog/, '');
            if (originalUrl !== window.location.href) {
                window.location.href = decodeURIComponent(originalUrl);
            }
        }
    };

    const langSelect = document.querySelector('.lang select');
    if (langSelect) {
        langSelect.addEventListener('change', function () {
            changeLang(this.value);
        });
    }

    const searchInput    = document.querySelector(".search input");
    const searchDropdown = document.getElementById("searchDropdown");
    const resultsList    = document.getElementById("searchResultsList");
    const searchLoading  = document.getElementById("searchLoading");
    const searchEmpty    = document.getElementById("searchEmpty");
    const resultsCount   = document.getElementById("resultsCount");
    const searchQuery    = document.getElementById("searchQuery");

    if (!searchInput) return;

    let searchTimeout = null;

    function openSearchDropdown() {
        searchDropdown.classList.add("active");
    }

    window.closeSearchDropdown = function () {
        searchDropdown.classList.remove("active");
        searchInput.value = "";
    };

    document.addEventListener("click", function (e) {
        if (!searchDropdown.contains(e.target) && !searchInput.contains(e.target)) {
            closeSearchDropdown();
        }
    });

    searchInput.addEventListener("input", function () {
        const query = this.value.trim();
        clearTimeout(searchTimeout);
        openSearchDropdown();
        showLoading();
        searchTimeout = setTimeout(() => {
            performSearch(query);
        }, 400);
    });

    searchInput.addEventListener("keydown", function (e) {
        if (e.key === "Escape") closeSearchDropdown();
        if (e.key === "Enter") {
            clearTimeout(searchTimeout);
            const query = this.value.trim();
            if (query.length >= 2) performSearch(query);
        }
    });

    function normalize(text) {
        return text.toLowerCase().trim();
    }

    function showLoading() {
        searchLoading.classList.add("active");
        searchEmpty.classList.remove("active");
        resultsList.innerHTML = "";
        resultsCount.textContent = "";
    }

    function showEmpty() {
        searchLoading.classList.remove("active");
        searchEmpty.classList.add("active");
        resultsList.innerHTML = "";
        resultsCount.textContent = "0 نتيجة";
    }

    function showResults(results, query) {
        searchLoading.classList.remove("active");
        searchEmpty.classList.remove("active");
        resultsCount.textContent = `${results.length} نتيجة`;
        searchQuery.textContent = query;
        renderResults(results, query);
    }

    function renderResults(results, query) {
        resultsList.innerHTML = "";
        results.forEach(item => {
            const el = document.createElement("a");
            el.className = "search-result-item";
            el.href = item.url || "#";
            const highlightedName = highlightText(item.name, query);
            el.innerHTML = `
                <div class="result-icon">
                    <i class="${getIcon(item.type)}"></i>
                </div>
                <div class="result-info">
                    <p class="result-name">${highlightedName}</p>
                    <p class="result-meta">${item.meta || ""}</p>
                </div>
                <span class="result-badge ${item.type}">${getBadgeLabel(item.type)}</span>
            `;
            resultsList.appendChild(el);
        });
    }

    function highlightText(text, query) {
        const regex = new RegExp(`(${query})`, "gi");
        return text.replace(regex, `<span class="highlight">$1</span>`);
    }

    function getIcon(type) {
        const icons = {
            doctor:  "fa-solid fa-user-doctor",
            nurse:   "fa-solid fa-user-nurse",
            service: "fa-solid fa-stethoscope",
        };
        return icons[type] || "fa-solid fa-magnifying-glass";
    }

    function getBadgeLabel(type) {
        const labels = {
            doctor:  "طبيب",
            nurse:   "ممرض",
            service: "خدمة",
        };
        return labels[type] || type;
    }

});