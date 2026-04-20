"use strict";

var STATIC_NOTIFICATIONS = [
    { id:1, unread:true,  type:"new-visit", title:"طلب زيارة جديد",   desc:"حسين التوني — 17 أبريل 11:30",        time:"منذ 5 دقائق",  visitRowId:"row-nv-1" },
    { id:2, unread:true,  type:"new-visit", title:"طلب زيارة جديد",   desc:"خالد مختار — 18 أبريل 11:00",         time:"منذ 12 دقيقة", visitRowId:"row-nv-2" },
    { id:3, unread:true,  type:"new-visit", title:"طلب زيارة جديد",   desc:"خالد محمود — 17 أبريل 10:30",         time:"منذ 1 ساعة",   visitRowId:"row-nv-3" },
    { id:4, unread:true,  type:"new-visit", title:"طلب زيارة جديد",   desc:"محمد أحمد — 17 أبريل 13:30",          time:"منذ 5 ساعات",  visitRowId:"row-nv-4" },
    { id:5, unread:false, type:"cancelled", title:"تم إلغاء الزيارة", desc:"نبيل محمود — ألغى موعده",              time:"أمس" },
    { id:6, unread:false, type:"info",      title:"تذكير بزيارة غداً",desc:"مازن محمد — غداً الساعة 12:00 ظهراً", time:"منذ يومين" }
];

var STATIC_CALENDAR_VISITS = [
    { date:"2026-04-16", time:"10:00", name:"محمد علي",       address:"القاهرة",      type:"كشف",    status:"confirmed" },
    { date:"2026-04-16", time:"11:00", name:"خالد رضا",       address:"مصر الجديدة",  type:"كشف",    status:"confirmed" },
    { date:"2026-04-16", time:"12:00", name:"مازن محمد",      address:"أكتوبر",       type:"متابعة", status:"confirmed" },
    { date:"2026-04-16", time:"17:00", name:"محمد أحمد",      address:"المعادي",      type:"كشف",    status:"confirmed" },
    { date:"2026-04-18", time:"09:00", name:"سامح جلال",      address:"الهرم",        type:"متابعة", status:"confirmed" },
    { date:"2026-04-20", time:"11:30", name:"نادي عبدالحميد", address:"الجيزة",       type:"كشف",    status:"confirmed" },
    { date:"2026-04-22", time:"08:00", name:"شهاب سيد",       address:"القاهرة",      type:"كشف",    status:"confirmed" }
];

document.addEventListener("DOMContentLoaded", function () {
    buildCalendarData();
    renderCalendar();
    initNotifications();
    initOutsideClick();
    showSection("dashboard");
    loadTheme();
    updatePendingStatCard();

    var avatarInput = document.getElementById("avatarInput");
    if (avatarInput) {
        avatarInput.addEventListener("change", function () {
            if (!this.files || !this.files[0]) return;
            var file = this.files[0];
            if (file.size > 5 * 1024 * 1024) { showProfileToast("حجم الصورة كبير جداً — الحد الأقصى 5 ميجابايت", "warning"); return; }
            var reader = new FileReader();
            reader.onload = function (e) {
                var img = document.getElementById("profileAvatarImg");
                if (img) img.src = e.target.result;
                var sideImg = document.querySelector(".image-profile img");
                if (sideImg) sideImg.src = e.target.result;
            };
            reader.readAsDataURL(file);
        });
    }

    initDocuments();

    document.querySelectorAll(".wh-row input[type='checkbox']").forEach(function (cb) {
        cb.addEventListener("change", function () {
            var row = this.closest(".wh-row");
            if (!row) return;
            row.querySelectorAll("input[type='time']").forEach(function (t) { t.disabled = !cb.checked; });
        });
    });

    var newPwd = document.getElementById("inputNewPwd");
    if (newPwd) newPwd.addEventListener("input", function () { updateStrengthBar(this.value); });
});

window.showSection = function (sectionId) {
    document.querySelectorAll("[id^='section-']").forEach(function(el){ el.style.display = "none"; });
    var target = document.getElementById("section-" + sectionId);
    if (target) target.style.display = "block";

    document.querySelectorAll(".nav-item").forEach(function(item){ item.classList.remove("active"); });
    var activeItem = document.querySelector(".nav-item[onclick=\"showSection('" + sectionId + "')\"]");
    if (activeItem) activeItem.classList.add("active");

    var nav  = document.querySelector(".nav");
    var hamb = document.querySelector(".hamburger");
    if (nav)  nav.classList.remove("active");
    if (hamb) hamb.classList.remove("open");
};


var _notifications = [];

function initNotifications() {
    _notifications = STATIC_NOTIFICATIONS.slice();
    renderNotifications();
}

function renderNotifications() {
    var list  = document.getElementById("notifList");
    var badge = document.getElementById("notifBadge");
    if (!list) return;

    var unreadCount = _notifications.filter(function(n){ return n.unread; }).length;
    if (badge) {
        badge.textContent = unreadCount;
        badge.classList.toggle("hidden", unreadCount === 0);
    }

    if (_notifications.length === 0) {
        list.innerHTML = '<div class="notif-empty"><i class="fa-solid fa-bell-slash"></i><p>لا توجد إشعارات</p></div>';
        return;
    }

    var iconMap = { "new-visit":"fa-calendar-plus", confirmed:"fa-circle-check", cancelled:"fa-circle-xmark", info:"fa-circle-info" };

    list.innerHTML = _notifications.map(function(n) {
        return '<div class="notif-item ' + (n.unread ? "unread" : "") + '" onclick="handleNotifClick(' + n.id + ')">'
            + '<div class="notif-icon ' + n.type + '"><i class="fa-solid ' + (iconMap[n.type] || "fa-bell") + '"></i></div>'
            + '<div class="notif-body">'
            +   '<div class="notif-title">' + n.title + '</div>'
            +   '<div class="notif-desc">'  + n.desc  + '</div>'
            +   '<div class="notif-time">'  + n.time  + '</div>'
            + '</div></div>';
    }).join("");
}

window.handleNotifClick = function (id) {
    var n = _notifications.find(function(x){ return x.id === id; });
    if (!n) return;
    n.unread = false;
    renderNotifications();
    closeNotif();

    if (n.type === "new-visit") {
        showSection("new-visits");
        if (n.visitRowId) {
            setTimeout(function() {
                var row = document.getElementById(n.visitRowId);
                if (row) {
                    row.scrollIntoView({ behavior:"smooth", block:"center" });
                    row.classList.add("highlight-row");
                    setTimeout(function() { row.classList.remove("highlight-row"); }, 2500);
                }
            }, 120);
        }
    } else if (n.type === "cancelled") {
        showSection("history");
        setTimeout(function() {
            var sf = document.getElementById("historyStatusFilter");
            if (sf) { sf.value = "cancelled"; filterHistoryTable(); }
        }, 120);
    } else if (n.type === "confirmed") {
        showSection("history");
    }

};

window.toggleNotif = function () {
    var dd = document.getElementById("notifDropdown");
    if (dd) dd.classList.toggle("open");
};

window.closeNotif = function () {
    var dd = document.getElementById("notifDropdown");
    if (dd) dd.classList.remove("open");
};

window.markRead = function (id) {
    var n = _notifications.find(function(x){ return x.id === id; });
    if (n) { n.unread = false; renderNotifications(); }
};

window.markAllRead = function () {
    _notifications.forEach(function(n){ n.unread = false; });
    renderNotifications();
};


var calVisits    = {};
var calYear      = new Date().getFullYear();
var calMonth     = new Date().getMonth();
var selectedDate = null;

function buildCalendarData() {
    STATIC_CALENDAR_VISITS.forEach(function(v){ addVisitToCalendar(v.date, v); });
}

function addVisitToCalendar(dateKey, visit) {
    if (!calVisits[dateKey]) calVisits[dateKey] = [];
    var exists = calVisits[dateKey].some(function(v){ return v.name === visit.name && v.time === visit.time; });
    if (!exists) calVisits[dateKey].push(visit);
}

function renderCalendar() {
    var grid  = document.getElementById("calGrid");
    var label = document.getElementById("calMonthLabel");
    if (!grid || !label) return;

    var months = ["يناير","فبراير","مارس","إبريل","مايو","يونيو","يوليو","أغسطس","سبتمبر","أكتوبر","نوفمبر","ديسمبر"];
    label.textContent = months[calMonth] + " " + calYear;

    var today       = new Date();
    var firstDay    = new Date(calYear, calMonth, 1).getDay();
    var daysInMonth = new Date(calYear, calMonth + 1, 0).getDate();

    grid.innerHTML = "";

    for (var i = 0; i < firstDay; i++) {
        var empty = document.createElement("div");
        empty.className = "cal-day empty";
        grid.appendChild(empty);
    }

    for (var d = 1; d <= daysInMonth; d++) {
        var dateKey = calYear + "-" + pad(calMonth + 1) + "-" + pad(d);
        var visits  = calVisits[dateKey] || [];
        var cell    = document.createElement("div");
        cell.className    = "cal-day";
        cell.dataset.date = dateKey;

        var isToday = (d === today.getDate() && calMonth === today.getMonth() && calYear === today.getFullYear());
        if (isToday)                  cell.classList.add("today");
        if (selectedDate === dateKey) cell.classList.add("selected");

        cell.innerHTML = "<span>" + d + "</span>";

        if (visits.length > 0) {
            cell.classList.add("has-visits");
            var dots = document.createElement("div");
            dots.className = "cal-dots";
            for (var j = 0; j < Math.min(visits.length, 3); j++) {
                var dot = document.createElement("div");
                dot.className = "cal-dot";
                dots.appendChild(dot);
            }
            cell.appendChild(dots);
            (function(dk){ cell.addEventListener("click", function(){ openDayDetail(dk); }); })(dateKey);
        }

        grid.appendChild(cell);
    }
}

function pad(n) { return String(n).padStart(2, "0"); }

function openDayDetail(dateKey) {
    selectedDate = dateKey;
    renderCalendar();

    var visits  = calVisits[dateKey] || [];
    var detail  = document.getElementById("calDayDetail");
    var titleEl = document.getElementById("calDetailTitle");
    var listEl  = document.getElementById("calDetailList");
    if (!detail || !titleEl || !listEl) return;

    var arabicDays   = ["الأحد","الاثنين","الثلاثاء","الأربعاء","الخميس","الجمعة","السبت"];
    var arabicMonths = ["يناير","فبراير","مارس","إبريل","مايو","يونيو","يوليو","أغسطس","سبتمبر","أكتوبر","نوفمبر","ديسمبر"];
    var parts   = dateKey.split("-");
    var dateObj = new Date(parseInt(parts[0]), parseInt(parts[1]) - 1, parseInt(parts[2]));
    titleEl.textContent = arabicDays[dateObj.getDay()] + " " + parseInt(parts[2]) + " " + arabicMonths[parseInt(parts[1]) - 1] + " " + parts[0];

    if (visits.length === 0) {
        listEl.innerHTML = '<div class="cal-no-visits">لا توجد زيارات في هذا اليوم</div>';
    } else {
        var sorted = visits.slice().sort(function(a,b){ return a.time.localeCompare(b.time); });
        listEl.innerHTML = sorted.map(function(v) {
            var statusClass = v.status === "confirmed" ? "confirmed" : (v.status === "cancelled" ? "cancelled" : "pending");
            var statusLabel = v.status === "confirmed" ? "مؤكدة"    : (v.status === "cancelled" ? "ملغية"    : "قيد الانتظار");
            return '<div class="cal-visit-item">'
                + '<div class="cal-visit-time">' + v.time + '</div>'
                + '<div class="cal-visit-info">'
                +   '<div class="cal-visit-name">' + v.name + '</div>'
                +   '<div class="cal-visit-meta"><span>' + v.type + '</span> - <span>' + v.address + '</span></div>'
                + '</div>'
                + '<span class="status-badge ' + statusClass + '">' + statusLabel + '</span>'
                + '</div>';
        }).join("");
    }

    detail.style.display = "block";
}

window.closeDayDetail = function () {
    selectedDate = null;
    var el = document.getElementById("calDayDetail");
    if (el) el.style.display = "none";
    renderCalendar();
};

window.prevMonth = function () {
    calMonth--;
    if (calMonth < 0) { calMonth = 11; calYear--; }
    selectedDate = null;
    var el = document.getElementById("calDayDetail");
    if (el) el.style.display = "none";
    renderCalendar();
};

window.nextMonth = function () {
    calMonth++;
    if (calMonth > 11) { calMonth = 0; calYear++; }
    selectedDate = null;
    var el = document.getElementById("calDayDetail");
    if (el) el.style.display = "none";
    renderCalendar();
};

window.acceptVisit = function (btn) {
    var row   = btn.closest("tr");
    var cells = row.querySelectorAll("td");

    var name      = cells[0] ? cells[0].textContent.trim() : "";
    var address   = cells[1] ? cells[1].textContent.trim() : "";
    var rawDate   = cells[2] ? cells[2].textContent.trim() : "";
    var phone     = cells[3] ? cells[3].textContent.trim() : "";
    var visitType = cells[4] ? cells[4].textContent.trim() : "";
    var condition = cells[5] ? cells[5].textContent.trim() : "";

    var dateKey = "", time = "";
    if (rawDate) {
        var parts = rawDate.split(" ");
        dateKey   = parts[0] || "";
        time      = parts[1] ? parts[1].substring(0, 5) : "";
    }

    var statusCell = row.querySelector(".status-badge");
    if (statusCell) { statusCell.className = "status-badge confirmed"; statusCell.textContent = "مؤكدة"; }

    var actionsCell = row.cells[6];
    if (actionsCell) actionsCell.innerHTML = '<span class="status-badge confirmed">تم القبول</span>';

    row.dataset.status = "confirmed";

    if (dateKey && name) {
        addVisitToCalendar(dateKey, { date:dateKey, time:time, name:name, address:address, type:visitType, status:"confirmed" });
        var dp = dateKey.split("-").map(Number);
        calYear = dp[0]; calMonth = dp[1] - 1;
        renderCalendar();
    }

    addToHistoryTable({ name:name, address:address, phone:phone, date:rawDate, dateKey:dateKey, diagnosis:condition, status:"confirmed" });

    updateNewVisitsBadge();
    updatePendingStatCard();

    showToast("✓ تمت تأكيد زيارة " + name);
};

window.rejectVisit = function (btn) {
    var row  = btn.closest("tr");
    var name = row.cells[0] ? row.cells[0].textContent.trim() : "المريض";

    var statusCell = row.querySelector(".status-badge");
    if (statusCell) { statusCell.className = "status-badge cancelled"; statusCell.textContent = "ملغية"; }

    var actionsCell = row.cells[6];
    if (actionsCell) actionsCell.innerHTML = '<span class="status-badge cancelled">تم الرفض</span>';

    row.dataset.status = "cancelled";
    updateNewVisitsBadge();
    updatePendingStatCard();

    showToast("✗ تم رفض زيارة " + name);
};

function addToHistoryTable(data) {
    var tbody = document.getElementById("history-tbody");
    if (!tbody) return;

    var tr = document.createElement("tr");
    tr.setAttribute("data-status", data.status);
    tr.setAttribute("data-date",   data.dateKey || "");

    var statusLabel = data.status === "confirmed" ? "مؤكدة" : "ملغية";
    var statusClass = data.status === "confirmed" ? "confirmed" : "cancelled";

    tr.innerHTML =
        '<td>—</td>' +
        '<td>' + (data.name    || "") + '</td>' +
        '<td>' + (data.address || "") + '</td>' +
        '<td>' + (data.phone   || "") + '</td>' +
        '<td>' + (data.date    || "") + '</td>' +
        '<td>' + (data.diagnosis || "—") + '</td>' +
        '<td><span class="status-badge ' + statusClass + '">' + statusLabel + '</span></td>';

    tbody.insertBefore(tr, tbody.firstChild);

    Array.from(tbody.querySelectorAll("tr")).forEach(function(r, idx) {
        var td = r.querySelector("td:first-child");
        if (td) td.textContent = idx + 1;
    });

    tr.classList.add("new-history-row");
    setTimeout(function() { tr.classList.remove("new-history-row"); }, 2500);
}


function updatePendingStatCard() {
    var count = document.querySelectorAll("#new-visits-tbody tr[data-status='pending']").length;
    var el    = document.getElementById("stat-pending-value");
    if (el) el.textContent = count;
}

function updateNewVisitsBadge() {
    var count = document.querySelectorAll("#new-visits-tbody tr[data-status='pending']").length;
    var badge = document.querySelector(".nav-item[onclick=\"showSection('new-visits')\"] .badge");
    if (badge) badge.textContent = count;
}

window.filterNewVisits = function (select) {
    var val  = select.value;
    var rows = document.querySelectorAll("#new-visits-tbody tr");
    rows.forEach(function(row){
        row.style.display = (!val || val === "all") ? "" : (row.dataset.status === val ? "" : "none");
    });
};

window.filterHistoryTable = function () {
    var searchVal = ((document.querySelector("#section-history .search-box input") || {}).value || "").trim().toLowerCase();
    var periodVal = (document.getElementById("historyPeriodFilter") || {}).value || "all";
    var statusVal = (document.getElementById("historyStatusFilter") || {}).value || "all";

    var rows    = document.querySelectorAll("#history-tbody tr");
    var now     = new Date();
    var visible = 0;

    rows.forEach(function(row){
        var text    = row.textContent.toLowerCase();
        var status  = row.dataset.status || "";
        var rawDate = row.dataset.date   || "";
        var rowDate = rawDate ? new Date(rawDate) : null;

        var matchSearch = !searchVal || text.includes(searchVal);
        var matchStatus = statusVal === "all" || status === statusVal;
        var matchPeriod = true;

        if (rowDate && periodVal !== "all") {
            var diffDays       = (now - rowDate) / (1000 * 60 * 60 * 24);
            var thisMonthStart = new Date(now.getFullYear(), now.getMonth(), 1);
            var lastMonthStart = new Date(now.getFullYear(), now.getMonth() - 1, 1);
            var lastMonthEnd   = new Date(now.getFullYear(), now.getMonth(), 0);
            if      (periodVal === "this-month")  matchPeriod = rowDate >= thisMonthStart;
            else if (periodVal === "last-month")  matchPeriod = rowDate >= lastMonthStart && rowDate <= lastMonthEnd;
            else if (periodVal === "3-months")    matchPeriod = diffDays <= 90;
        }

        var show = matchSearch && matchStatus && matchPeriod;
        row.style.display = show ? "" : "none";
        if (show) visible++;
    });

    var emptyEl = document.getElementById("historyEmpty");
    if (emptyEl) emptyEl.style.display = visible === 0 ? "" : "none";
};

window.filterTable = function (input, tbodyId) {
    var query = input.value.trim().toLowerCase();
    var rows  = document.querySelectorAll("#" + tbodyId + " tr");
    rows.forEach(function(row){
        row.style.display = row.textContent.toLowerCase().includes(query) ? "" : "none";
    });
};

function loadTheme() {
    if (localStorage.getItem("theme") === "dark") {
        document.body.classList.add("dark-mode");
        document.querySelectorAll(".theme-toggle i, #navThemeIcon").forEach(function(i){ i.className = "fa-solid fa-sun"; });
    }
}

window.toggleTheme = function () {
    document.body.classList.toggle("dark-mode");
    var isDark = document.body.classList.contains("dark-mode");
    document.querySelectorAll(".theme-toggle i, #navThemeIcon").forEach(function(i){
        i.className = isDark ? "fa-solid fa-sun" : "fa-solid fa-moon";
    });
    localStorage.setItem("theme", isDark ? "dark" : "light");
};


window.logout = function () {
    if (!confirm("هل تريد تسجيل الخروج؟")) return;
    window.location.href = "index.html";
};

window.toggleMenu = function () {
    var nav  = document.querySelector(".nav");
    var hamb = document.querySelector(".hamburger");
    if (nav)  nav.classList.toggle("active");
    if (hamb) hamb.classList.toggle("open");
};


function initOutsideClick() {
    document.addEventListener("click", function(e){
        var wrapper = document.getElementById("notifWrapper");
        var dd      = document.getElementById("notifDropdown");
        if (dd && wrapper && !wrapper.contains(e.target)) dd.classList.remove("open");
    });
}

document.addEventListener("keydown", function(e){ if (e.key === "Escape") closeNotif(); });

function showToast(msg) {
    var toast = document.getElementById("calToast");
    if (!toast) {
        toast = document.createElement("div");
        toast.id = "calToast"; toast.className = "cal-toast";
        document.body.appendChild(toast);
    }
    toast.textContent = msg;
    toast.classList.add("show");
    setTimeout(function(){ toast.classList.remove("show"); }, 3500);
}


window.switchProfileTab = function (tabId, btn) {
    document.querySelectorAll(".profile-tab").forEach(function(t){ t.classList.remove("active"); });
    document.querySelectorAll(".profile-tab-content").forEach(function(c){ c.classList.remove("active"); });
    if (btn) btn.classList.add("active");
    var content = document.getElementById("tab-" + tabId);
    if (content) content.classList.add("active");
};

window.saveProfileSection = function (e, section) {
    e.preventDefault();
    var formId = section === "personal" ? "personalForm" : section === "professional" ? "professionalForm" : "securityForm";
    var form   = document.getElementById(formId);
    if (!form) return;
    clearProfileErrors(form);

    if (section === "security") {
        var np = document.getElementById("inputNewPwd");
        var cp = document.getElementById("inputConfirmPwd");
        if (np && cp && np.value !== cp.value) { showProfileFieldError(cp, "كلمتا المرور غير متطابقتين"); return; }
    }

    if (section === "professional") {
        var valid = true;
        document.querySelectorAll(".wh-row").forEach(function(row){
            var cb = row.querySelector("input[type='checkbox']");
            if (!cb || !cb.checked) return;
            var times = row.querySelectorAll("input[type='time']");
            if (times.length < 2) return;
            if (times[0].value && times[1].value && times[0].value >= times[1].value) {
                times[1].classList.add("input-error"); valid = false;
            }
        });
        if (!valid) { showProfileToast("تأكد أن وقت الانتهاء بعد وقت البداية", "warning"); return; }
    }

    if (section === "personal") {
        var nameEl = document.getElementById("inputFullName");
        if (nameEl) {
            var hn = document.getElementById("heroName");
            var sn = document.querySelector(".name-profile");
            if (hn) hn.textContent = nameEl.value;
            if (sn) sn.textContent = nameEl.value;
        }
    }
    if (section === "professional") {
        var specEl = document.getElementById("inputSpecialty");
        if (specEl) { var hs = document.getElementById("heroSpec"); if (hs) hs.textContent = specEl.value; }
    }

    showProfileToast("تم حفظ التعديلات بنجاح ✓");
};

function showProfileFieldError(input, msg) {
    if (!input) return;
    input.classList.add("input-error");
    var errEl = document.getElementById("err-" + (input.name || ""));
    if (errEl) errEl.textContent = msg;
}

function clearProfileErrors(form) {
    if (!form) return;
    form.querySelectorAll(".input-error").forEach(function(el){ el.classList.remove("input-error"); });
    form.querySelectorAll(".pf-error").forEach(function(el){ el.textContent = ""; });
}

function showProfileToast(msg, type) {
    var existing = document.querySelectorAll(".dp-toast");
    existing.forEach(function(t){ t.remove(); });
    var colors = { success:"#128CCF", warning:"#e6a817", error:"#e53838" };
    var toast  = document.createElement("div");
    toast.className = "dp-toast";
    toast.textContent = msg;
    toast.style.cssText = [
        "position:fixed","bottom:28px","left:50%",
        "transform:translateX(-50%) translateY(16px)",
        "background:" + (colors[type] || colors.success),
        "color:white","padding:11px 26px","border-radius:30px",
        "font-size:13px","font-weight:700","font-family:'Cairo',sans-serif",
        "box-shadow:0 6px 20px rgba(0,0,0,0.18)","z-index:99999",
        "opacity:0","transition:all 0.3s ease","white-space:nowrap","pointer-events:none"
    ].join(";");
    document.body.appendChild(toast);
    requestAnimationFrame(function(){
        toast.style.opacity = "1"; toast.style.transform = "translateX(-50%) translateY(0)";
    });
    setTimeout(function(){
        toast.style.opacity = "0"; toast.style.transform = "translateX(-50%) translateY(16px)";
        setTimeout(function(){ toast.remove(); }, 300);
    }, 3000);
}

function updateStrengthBar(val) {
    var fill  = document.getElementById("strengthFill");
    var label = document.getElementById("strengthLabel");
    if (!fill || !label) return;
    var score = 0;
    if (val.length >= 8)          score++;
    if (/[A-Z]/.test(val))        score++;
    if (/[0-9]/.test(val))        score++;
    if (/[^A-Za-z0-9]/.test(val)) score++;
    var map = [
        { pct:"0%",   color:"#eee",    text:"",          textColor:"" },
        { pct:"25%",  color:"#e53838", text:"ضعيفة",     textColor:"#e53838" },
        { pct:"50%",  color:"#ff9800", text:"متوسطة",    textColor:"#ff9800" },
        { pct:"75%",  color:"#128CCF", text:"جيدة",      textColor:"#128CCF" },
        { pct:"100%", color:"#28a745", text:"قوية جداً", textColor:"#28a745" }
    ];
    fill.style.width = map[score].pct; fill.style.background = map[score].color;
    label.textContent = map[score].text; label.style.color = map[score].textColor;
}

window.togglePwd = function (inputId, btn) {
    var input = document.getElementById(inputId);
    if (!input) return;
    var isText = input.type === "text";
    input.type = isText ? "password" : "text";
    var icon = btn.querySelector("i");
    if (icon) icon.className = isText ? "fa-solid fa-eye" : "fa-solid fa-eye-slash";
};

window.addTagOnEnter = function (e) {
    if (e.key !== "Enter") return;
    e.preventDefault();
    var input = document.getElementById("tagInput");
    if (!input) return;
    var val = input.value.trim();
    if (!val) return;
    var wrap = document.getElementById("serviceAreasTags");
    if (!wrap) return;
    var existing = Array.from(wrap.querySelectorAll(".pf-tag"))
        .map(function(t){ return t.textContent.replace("×","").trim(); });
    if (existing.includes(val)) { input.value = ""; return; }
    var tag = document.createElement("span");
    tag.className = "pf-tag";
    tag.innerHTML = val + ' <button type="button" onclick="removeTag(this)">×</button>';
    wrap.insertBefore(tag, input);
    input.value = "";
    updateServiceAreasHidden();
};

window.removeTag = function (btn) {
    var tag = btn.closest(".pf-tag");
    if (tag) { tag.remove(); updateServiceAreasHidden(); }
};

function updateServiceAreasHidden() {
    var wrap   = document.getElementById("serviceAreasTags");
    var hidden = document.getElementById("serviceAreasHidden");
    if (!wrap || !hidden) return;
    hidden.value = Array.from(wrap.querySelectorAll(".pf-tag"))
        .map(function(t){ return t.textContent.replace("×","").trim(); }).join(",");
}

function getAntiForgeryToken() {
    var el = document.querySelector('input[name="__RequestVerificationToken"]');
    return el ? el.value : "";
}



var DOCTOR_DOCS = {
    cert          : false,  
    nationalId    : false,
    CertUrl       : null,   
    NationalIdFrontUrl : null,
    NationalIdBackUrl  : null
};

function initDocuments() {
    updateDocStatus("docCert",       DOCTOR_DOCS.cert,       false);
    updateDocStatus("docNationalId", DOCTOR_DOCS.nationalId, true);

    if (DOCTOR_DOCS.cert)       showDocViewBtn("certViewBtn");
    if (DOCTOR_DOCS.nationalId) showDocViewBtn("idViewBtn");

    var certInput = document.getElementById("certInput");
    if (certInput) {
        certInput.addEventListener("change", function () {
            if (!this.files || !this.files[0]) return;
            var file = this.files[0];
            if (file.size > 5 * 1024 * 1024) {
                showProfileToast("حجم الملف كبير جداً — الحد الأقصى 5 ميجابايت", "warning");
                this.value = ""; return;
            }

            var isPdf = file.type === "application/pdf" || file.name.toLowerCase().endsWith(".pdf");

            if (isPdf) {
                DOCTOR_DOCS.CertUrl  = URL.createObjectURL(file);
                DOCTOR_DOCS.certIsPdf = true;
            } else {
                var reader = new FileReader();
                reader.onload = function (e) {
                    DOCTOR_DOCS.CertUrl   = e.target.result;
                    DOCTOR_DOCS.certIsPdf = false;
                };
                reader.readAsDataURL(file);
            }

            DOCTOR_DOCS.cert = true;
            updateDocStatus("docCert", true, false);
            showDocViewBtn("certViewBtn");
            showProfileToast("تم رفع الشهادة الطبية بنجاح ✓");
            this.value = "";

        });
    }

    var idInput = document.getElementById("idCardInput");
    if (idInput) {
        idInput.addEventListener("change", function () {
            var files = this.files;
            if (!files || files.length === 0) return;

            if (files.length < 2) {
                showProfileToast(
                    "يرجى اختيار صورتي البطاقة الوجه و الضهر معاً).",
                    "warning"
                );
                this.value = ""; return;
            }

            for (var i = 0; i < 2; i++) {
                if (files[i].size > 5 * 1024 * 1024) {
                    showProfileToast(files[i].name + " — حجم الملف أكبر من 5 ميجابايت", "warning");
                    this.value = ""; return;
                }
            }

            var rFront = new FileReader();
            rFront.onload = function (e) {
                DOCTOR_DOCS.NationalIdFrontUrl = e.target.result;
            };
            rFront.readAsDataURL(files[0]);

            var rBack = new FileReader();
            rBack.onload = function (e) {
                DOCTOR_DOCS.NationalIdBackUrl = e.target.result;
                DOCTOR_DOCS.nationalId = true;
                updateDocStatus("docNationalId", true, true);
                showDocViewBtn("idViewBtn");
                showProfileToast("تم تحديث بطاقة الرقم القومي (وجه + ظهر) بنجاح ✓");
            };
            rBack.readAsDataURL(files[1]);

            this.value = "";

        });
    }
}

function updateDocStatus(docId, isUploaded, isDual) {
    var el = document.getElementById(docId);
    if (!el) return;
    if (isUploaded) {
        el.className = "doc-status uploaded";
        el.innerHTML = "<i class=\"fa-solid fa-circle-check\"></i> " +
                       (isDual ? "تم الرفع (وجه + ظهر)" : "تم الرفع");
    } else {
        el.className = "doc-status missing";
        el.innerHTML = "<i class=\"fa-solid fa-circle-xmark\"></i> لم يرفع بعد";
    }
}

function showDocViewBtn(btnId) {
    var btn = document.getElementById(btnId);
    if (btn) btn.style.display = "";
}

window.viewCert = function () {
    var url    = DOCTOR_DOCS.CertUrl;
    var isPdf  = DOCTOR_DOCS.certIsPdf;
    setDocModalTitle("الشهادة الطبية");

    if (!url) {
        setDocModalBody("<div style=\"text-align:center;padding:30px;color:#bbb;\"><i class=\"fa-solid fa-file-medical\" style=\"font-size:40px;display:block;margin-bottom:10px;\"></i><p>لا يوجد ملف مرفوع</p></div>");
    } else if (isPdf) {
        setDocModalBody(
            "<div class=\"cert-view-pdf\">" +
            "<i class=\"fa-solid fa-file-pdf\"></i>" +
            "<p>ملف PDF — انقر للفتح في نافذة جديدة</p>" +
            "<a href=\"" + url + "\" target=\"_blank\"><i class=\"fa-solid fa-external-link-alt\"></i> فتح الملف</a>" +
            "</div>"
        );
    } else {
        setDocModalBody("<div class=\"cert-view\"><img src=\"" + url + "\" alt=\"الشهادة الطبية\"></div>");
    }
    openDocModal();
};

window.viewNationalId = function () {
    var front = DOCTOR_DOCS.NationalIdFrontUrl;
    var back  = DOCTOR_DOCS.NationalIdBackUrl;
    setDocModalTitle("بطاقة الرقم القومي");
    setDocModalBody(
        "<div class=\"id-cards-wrap\">" +
        "<div class=\"id-card-side\"><label>الوجه</label>" +
        (front ? "<img src=\"" + front + "\" alt=\"وجه البطاقة\">" : "<p style=\"color:#bbb;text-align:center;\">لا توجد صورة</p>") +
        "</div>" +
        "<div class=\"id-card-side\"><label>الظهر</label>" +
        (back  ? "<img src=\"" + back  + "\" alt=\"ظهر البطاقة\">"  : "<p style=\"color:#bbb;text-align:center;\">لا توجد صورة</p>") +
        "</div>" +
        "</div>"
    );
    openDocModal();
};

function openDocModal() {
    var overlay = document.getElementById("docModalOverlay");
    if (overlay) { overlay.classList.add("open"); document.body.style.overflow = "hidden"; }
}

window.closeDocModal = function (e) {
    if (e && e.target !== document.getElementById("docModalOverlay")) return;
    var overlay = document.getElementById("docModalOverlay");
    if (overlay) { overlay.classList.remove("open"); document.body.style.overflow = ""; }
};

function setDocModalTitle(title) {
    var el = document.getElementById("docModalTitle");
    if (el) el.textContent = title;
}

function setDocModalBody(html) {
    var el = document.getElementById("docModalBody");
    if (el) el.innerHTML = html;
}

document.addEventListener("keydown", function (e) {
    if (e.key === "Escape") {
        var overlay = document.getElementById("docModalOverlay");
        if (overlay && overlay.classList.contains("open")) {
            overlay.classList.remove("open");
            document.body.style.overflow = "";
        }
    }
});