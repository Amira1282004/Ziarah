"use strict";

var CURRENT_USER = {
    FirstName    : "محمد",
    LastName     : "أحمد",
    FullName     : "محمد أحمد",
    Phone        : "01012345678",
    Email        : "user@gmail.com",
    Birthdate    : "1990-05-15",
    Gender       : "male",
    Address      : "بني سويف - شارع الجمهوري",
    AvatarUrl    : "assets/image/be7d92ded7de75fe6dc9fd4406e789ac22393930.png",

    HasInsurance   : true,
    InsuranceNumber: "BS-2024-00471",
    InsuranceIssue : "2024-01-10",
    InsuranceExpiry: "2026-01-09",

    Documents: {
        nationalId    : true,
        insuranceCard : true,
        medicalReports: false
    },

    NationalIdFrontUrl : "assets/image/بطاقة الرقم القومي1.jpg",
    NationalIdBackUrl  : "assets/image/بطاقة الرقم القومي2.jpg",
    InsuranceCardUrl   : "assets/image/كارنيه الـتأمين.jpg",
    MedicalReports     : []
};


document.addEventListener("DOMContentLoaded", function () {
    initUserMenuClose();
    populateProfileData();
    initTabSwitching();
    initPasswordStrength();
    initFileUploads();
    initInsuranceSection();
    initDocuments();
    initForms();
    initAvatarUpload();
});



function initUserMenuClose() {
    document.addEventListener("click", function (e) {
        var userMenu = document.querySelector(".user-menu");
        var userDD   = document.getElementById("userDropdown");
        if (userDD && userMenu && !userMenu.contains(e.target)) {
            userDD.classList.remove("open");
        }
    });
}

window.toggleUserMenu = function () {
    var dd = document.getElementById("userDropdown");
    if (dd) dd.classList.toggle("open");
};

function populateProfileData() {
    var u = CURRENT_USER;

    setText("heroName",    u.FullName);
    setText("heroPhone",   u.Phone);
    setText("heroEmail",   u.Email);
    setText("heroInsStatus", u.HasInsurance ? "تأمين مسجل ✓" : "لا يوجد تأمين");

    var heroAvatar = document.getElementById("heroAvatar");
    if (heroAvatar && u.AvatarUrl) {
        heroAvatar.src = u.AvatarUrl;
    }

    setText("userName", u.FullName);

    setVal("firstName", u.FirstName);
    setVal("lastName",  u.LastName);
    setVal("phone",     u.Phone);
    setVal("email",     u.Email);
    setVal("birthdate", u.Birthdate);
    setVal("address",   u.Address);

    var genderSelect = document.getElementById("gender");
    if (genderSelect) genderSelect.value = u.Gender || "";
}

function setText(id, val) {
    var el = document.getElementById(id);
    if (el) el.textContent = val || "";
}

function setVal(id, val) {
    var el = document.getElementById(id);
    if (el) el.value = val || "";
}



function initTabSwitching() {

}

window.switchTab = function (tabName, btn) {
    document.querySelectorAll(".profile-panel").forEach(function (p) {
        p.classList.remove("active");
    });

    document.querySelectorAll(".profile-tab").forEach(function (b) {
        b.classList.remove("active");
    });

    var panel = document.getElementById("tab-" + tabName);
    if (panel) panel.classList.add("active");

    if (btn) btn.classList.add("active");
};



function initForms() {
    var personalForm  = document.getElementById("personalForm");
    var securityForm  = document.getElementById("securityForm");
    var insForm       = document.getElementById("insForm");

    if (personalForm)  personalForm.addEventListener("submit",  savePersonal);
    if (securityForm)  securityForm.addEventListener("submit",  savePassword);
    if (insForm)       insForm.addEventListener("submit",       saveInsurance);
}

window.savePersonal = function (e) {
    e.preventDefault();
    var form = document.getElementById("personalForm");
    if (!form) return;

    clearErrors(form);
    var valid = true;

    var phone = form.elements["phone"];
    if (phone && !/^01[0-9]{9}$/.test(phone.value.trim())) {
        showFieldError(phone, "رقم الهاتف غير صحيح (11 رقم يبدأ بـ 01)");
        valid = false;
    }

    var email = form.elements["email"];
    if (email && email.value.trim() && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.value.trim())) {
        showFieldError(email, "البريد الإلكتروني غير صحيح");
        valid = false;
    }

    if (!valid) return;

    var payload = {
        FirstName : form.elements["firstName"].value.trim(),
        LastName  : form.elements["lastName"].value.trim(),
        Phone     : phone.value.trim(),
        Email     : email.value.trim(),
        Birthdate : form.elements["birthdate"].value,
        Gender    : form.elements["gender"].value,
        Address   : form.elements["address"].value.trim()
    };

    CURRENT_USER.FirstName = payload.FirstName;
    CURRENT_USER.LastName  = payload.LastName;
    CURRENT_USER.FullName  = payload.FirstName + " " + payload.LastName;
    CURRENT_USER.Phone     = payload.Phone;
    CURRENT_USER.Email     = payload.Email;
    CURRENT_USER.Birthdate = payload.Birthdate;
    CURRENT_USER.Gender    = payload.Gender;
    CURRENT_USER.Address   = payload.Address;

    setText("heroName",  CURRENT_USER.FullName);
    setText("heroPhone", CURRENT_USER.Phone);
    setText("heroEmail", CURRENT_USER.Email);
    setText("userName",  CURRENT_USER.FullName);

    showToast("تم حفظ المعلومات الشخصية بنجاح");

};


window.savePassword = function (e) {
    e.preventDefault();
    var form = document.getElementById("securityForm");
    if (!form) return;

    clearErrors(form);
    var valid   = true;
    var current = document.getElementById("currentPass");
    var newPass = document.getElementById("newPass");
    var confirm = document.getElementById("confirmPass");

    if (!current || !current.value.trim()) {
        showFieldError(current, "يرجى إدخال كلمة المرور الحالية");
        valid = false;
    }

    if (!newPass || newPass.value.length < 8) {
        showFieldError(newPass, "كلمة المرور يجب أن تكون 8 أحرف على الأقل");
        valid = false;
    }

    if (confirm && newPass && confirm.value !== newPass.value) {
        showFieldError(confirm, "كلمة المرور غير متطابقة");
        valid = false;
    }

    if (!valid) return;

    var payload = {
        CurrentPassword : current.value,
        NewPassword     : newPass.value
    };

    form.reset();
    resetPasswordUI();
    showToast("تم تحديث كلمة المرور بنجاح");

};

function resetPasswordUI() {
    var fill  = document.getElementById("strengthFill");
    var label = document.getElementById("strengthLabel");
    if (fill)  { fill.style.width = "0%"; fill.style.background = ""; }
    if (label)  label.textContent = "";

    ["rule-len", "rule-upper", "rule-lower", "rule-num", "rule-special"].forEach(function (id) {
        var li = document.getElementById(id);
        if (!li) return;
        li.classList.remove("valid");
        var icon = li.querySelector("i");
        if (icon) { icon.className = "fa-solid fa-circle-xmark"; }
    });
}

window.togglePass = function (inputId, iconEl) {
    var input = document.getElementById(inputId);
    if (!input) return;

    if (input.type === "password") {
        input.type = "text";
        iconEl.classList.replace("fa-eye", "fa-eye-slash");
    } else {
        input.type = "password";
        iconEl.classList.replace("fa-eye-slash", "fa-eye");
    }
};


function initPasswordStrength() {
    var newPass = document.getElementById("newPass");
    if (!newPass) return;

    newPass.addEventListener("input", function () {
        evaluatePasswordStrength(newPass.value);
    });
}

function evaluatePasswordStrength(password) {
    var rules = {
        len     : password.length >= 8,
        upper   : /[A-Z]/.test(password),
        lower   : /[a-z]/.test(password),
        num     : /[0-9]/.test(password),
        special : /[!@#$%^&*()_+\-=\[\]{}|;':",.<>?/\\`~]/.test(password)
    };

    Object.keys(rules).forEach(function (key) {
        var li   = document.getElementById("rule-" + key);
        var icon = li ? li.querySelector("i") : null;
        if (!li || !icon) return;

        if (rules[key]) {
            li.classList.add("valid");
            icon.className = "fa-solid fa-circle-check";
        } else {
            li.classList.remove("valid");
            icon.className = "fa-solid fa-circle-xmark";
        }
    });

    var score = Object.values(rules).filter(Boolean).length;
    var fill  = document.getElementById("strengthFill");
    var label = document.getElementById("strengthLabel");

    var levels = [
        { pct: "0%",   color: "",        text: "" },
        { pct: "20%",  color: "#e53838", text: "ضعيفة جداً" },
        { pct: "40%",  color: "#f97316", text: "ضعيفة" },
        { pct: "60%",  color: "#eab308", text: "متوسطة" },
        { pct: "80%",  color: "#3b82f6", text: "جيدة" },
        { pct: "100%", color: "#16a34a", text: "قوية جداً" }
    ];

    var level = levels[score] || levels[0];
    if (fill)  { fill.style.width = level.pct; fill.style.background = level.color; }
    if (label) { label.textContent = level.text; label.style.color = level.color; }
}

function initInsuranceSection() {
    var u    = CURRENT_USER;
    var card = document.getElementById("insStatusCard");
    var wrap = document.getElementById("insFormWrap");

    if (u.HasInsurance) {
        renderInsuranceCard(u);
    } else {
        renderNoInsurance();
    }

    setVal("insNumber", u.InsuranceNumber || "");
    setVal("insIssue",  u.InsuranceIssue  || "");
    setVal("insExpiry", u.InsuranceExpiry || "");
}



function renderInsuranceCard(u) {
    var card = document.getElementById("insStatusCard");
    if (!card) return;
    var imgUrl = u.InsuranceCardUrl || "assets/image/كارنيه الـتأمين.jpg";
    card.innerHTML =
        "<div class=\"ins-status-icon has-ins\"><i class=\"fa-solid fa-shield-halved\"></i></div>" +
        "<div class=\"ins-status-text\"><h3 id=\"insStatusTitle\">التأمين مسجل</h3></div>" +
        "<div class=\"ins-status-img\"><img id=\"insCardImg\" src=\"" + imgUrl + "\" alt=\"كارنيه التأمين\"></div>";
}

function renderNoInsurance() {
    var card = document.getElementById("insStatusCard");
    if (!card) return;
    card.innerHTML =
        "<div class=\"ins-status-icon\"><i class=\"fa-solid fa-shield-halved\"></i></div>" +
        "<div class=\"ins-status-text\"><h3 id=\"insStatusTitle\">لا يوجد تأمين مسجل</h3></div>";
}

window.toggleInsForm = function () {
    var wrap = document.getElementById("insFormWrap");
    if (!wrap) return;
    wrap.style.display = wrap.style.display === "none" ? "" : "none";
};


function formatDate(dateStr) {
    if (!dateStr) return "—";
    try {
        var d = new Date(dateStr);
        return d.toLocaleDateString("ar-EG", { year: "numeric", month: "long", day: "numeric" });
    } catch (e) {
        return dateStr;
    }
}



function initDocuments() {
    var docs = CURRENT_USER.Documents || {};

    updateDocStatus("docNationalId",     docs.nationalId,     true);
    updateDocStatus("docInsuranceCard",  docs.insuranceCard,  false);
    updateDocStatus("docMedicalReports", docs.medicalReports, false);

    var idFilesInput = document.getElementById("newIdFiles");

    if (idFilesInput) {
        idFilesInput.addEventListener("change", function () {
            var files = this.files;
            if (!files || files.length === 0) return;

            if (files.length < 2) {
                alert("يرجى اختيار صورتين: الوجه والظهر معاً (اضغط Ctrl أو Shift لاختيار أكثر من صورة).");
                this.value = "";
                return;
            }

            for (var i = 0; i < 2; i++) {
                if (files[i].size > 5 * 1024 * 1024) {
                    alert(files[i].name + " — حجم الملف كبير جداً. الحد الأقصى 5 ميجابايت.");
                    this.value = "";
                    return;
                }
            }

            var readerFront = new FileReader();
            readerFront.onload = function (e) {
                CURRENT_USER.NationalIdFrontUrl = e.target.result;
            };
            readerFront.readAsDataURL(files[0]);

            var readerBack = new FileReader();
            readerBack.onload = function (e) {
                CURRENT_USER.NationalIdBackUrl = e.target.result;
                CURRENT_USER.Documents.nationalId = true;
                updateDocStatus("docNationalId", true, true);
                showToast("تم تحديث بطاقة الرقم القومي (وجه + ظهر) بنجاح ✓");
            };
            readerBack.readAsDataURL(files[1]);

            this.value = "";
        });
    }

    var insInput = document.getElementById("newInsFile");
    if (insInput) {
        insInput.addEventListener("change", function () {
            if (!this.files || !this.files[0]) return;
            if (this.files[0].size > 5 * 1024 * 1024) {
                alert("حجم الملف كبير جداً. الحد الأقصى 5 ميجابايت."); this.value = ""; return;
            }
            var reader = new FileReader();
            reader.onload = function (e) {
                CURRENT_USER.InsuranceCardUrl = e.target.result;

                var insImg = document.getElementById("insCardImg") || document.querySelector(".ins-status-img img");
                if (insImg) {
                    insImg.src = e.target.result;
                } else {
                    var insImgWrap = document.querySelector(".ins-status-img");
                    if (insImgWrap) {
                        var newImg = document.createElement("img");
                        newImg.id  = "insCardImg";
                        newImg.src = e.target.result;
                        newImg.alt = "كارنيه التأمين";
                        insImgWrap.innerHTML = "";
                        insImgWrap.appendChild(newImg);
                    }
                }

                CURRENT_USER.Documents.insuranceCard = true;
                updateDocStatus("docInsuranceCard", true, false);
                showToast("تم تحديث كارنيه التأمين — وتم التحديث في سكشن التأمين ✓");
            };
            reader.readAsDataURL(this.files[0]);
        });
    }

    var medInput = document.getElementById("newMedFiles");
    if (medInput) {
        medInput.addEventListener("change", function () {
            if (!this.files || !this.files.length) return;
            var added = 0;
            Array.from(this.files).forEach(function (file) {
                if (file.size > 20 * 1024 * 1024) {
                    alert(file.name + " أكبر من 20 ميجابايت — تم تجاهله"); return;
                }
                var type = getFileIcon(file.name);
                var url  = URL.createObjectURL(file);
                CURRENT_USER.MedicalReports.push({
                    name: file.name,
                    size: formatFileSize(file.size),
                    type: type,
                    url : url
                });
                added++;
            });
            this.value = "";
            if (added > 0) {
                CURRENT_USER.Documents.medicalReports = true;
                updateDocStatus("docMedicalReports", true, false);
                var viewBtn = document.getElementById("medViewBtn");
                if (viewBtn) viewBtn.style.display = "";
                showToast("تم رفع " + added + " ملف بنجاح ✓");
            }
        });
    }

    var viewBtn = document.getElementById("medViewBtn");
    if (viewBtn && CURRENT_USER.MedicalReports && CURRENT_USER.MedicalReports.length > 0) {
        viewBtn.style.display = "";
    }
}

function updateDocStatus(docId, isUploaded, isDual) {
    var el = document.getElementById(docId);
    if (!el) return;
    if (isUploaded) {
        el.className = "doc-status uploaded";
        el.innerHTML = "<i class=\"fa-solid fa-circle-check\"></i> " + (isDual ? "تم الرفع (وجه + ظهر)" : "تم الرفع");
    } else {
        el.className = "doc-status missing";
        el.innerHTML = "<i class=\"fa-solid fa-circle-xmark\"></i> لم يرفع بعد";
    }
}

window.viewNationalId = function () {
    var front = CURRENT_USER.NationalIdFrontUrl || "assets/image/بطاقة الرقم القومي1.jpg";
    var back  = CURRENT_USER.NationalIdBackUrl  || "assets/image/بطاقة الرقم القومي2.jpg";
    setModalTitle("بطاقة الرقم القومي");
    setModalBody(
        "<div class=\"id-cards-wrap\">" +
        "<div class=\"id-card-side\"><label>الوجه</label><img src=\"" + front + "\" alt=\"وجه البطاقة\"></div>" +
        "<div class=\"id-card-side\"><label>الظهر</label><img src=\"" + back  + "\" alt=\"ظهر البطاقة\"></div>" +
        "</div>"
    );
    openDocModal();
};

window.viewInsuranceCard = function () {
    var url = CURRENT_USER.InsuranceCardUrl || "assets/image/كارنيه الـتأمين.jpg";
    setModalTitle("كارنيه التأمين");
    setModalBody(
        "<div class=\"ins-card-view\"><img src=\"" + url + "\" alt=\"كارنيه التأمين\" style=\"max-width:100%; border-radius:12px;\"></div>"
    );
    openDocModal();
};

window.viewMedicalReports = function () {
    var reports = CURRENT_USER.MedicalReports || [];
    setModalTitle("التقارير الطبية (" + reports.length + ")");

    if (!reports.length) {
        setModalBody("<div class=\"med-empty\"><i class=\"fa-solid fa-folder-open\"></i><p>لا توجد ملفات مرفوعة</p></div>");
        openDocModal(); return;
    }

    var html = "<div class=\"med-files-list\">";
    reports.forEach(function (f, idx) {
        html +=
            "<div class=\"med-file-item\">" +
            "<i class=\"fa-solid " + f.type + " " + getIconClass(f.type) + "\"></i>" +
            "<span class=\"med-file-name\">" + f.name + "</span>" +
            "<span class=\"med-file-size\">" + f.size + "</span>" +
            "<button class=\"med-file-del\" onclick=\"deleteMedFile(" + idx + ")\" title=\"حذف\">" +
            "<i class=\"fa-solid fa-trash-can\"></i></button>" +
            "</div>";
    });
    html += "</div>";
    setModalBody(html);
    openDocModal();
};

window.deleteMedFile = function (idx) {
    if (!confirm("هل تريد حذف هذا الملف؟")) return;
    CURRENT_USER.MedicalReports.splice(idx, 1);
    if (CURRENT_USER.MedicalReports.length === 0) {
        CURRENT_USER.Documents.medicalReports = false;
        updateDocStatus("docMedicalReports", false, false);
        var viewBtn = document.getElementById("medViewBtn");
        if (viewBtn) viewBtn.style.display = "none";
        closeDocModal();
    } else {
        viewMedicalReports();
    }
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

document.addEventListener("keydown", function (e) {
    if (e.key === "Escape") {
        var overlay = document.getElementById("docModalOverlay");
        if (overlay) { overlay.classList.remove("open"); document.body.style.overflow = ""; }
    }
});

function setModalTitle(title) {
    var el = document.getElementById("docModalTitle");
    if (el) el.textContent = title;
}

function setModalBody(html) {
    var el = document.getElementById("docModalBody");
    if (el) el.innerHTML = html;
}

function getFileIcon(filename) {
    var ext = filename.split(".").pop().toLowerCase();
    if (["jpg","jpeg","png","gif","webp"].includes(ext)) return "fa-image";
    if (ext === "pdf") return "fa-file-pdf";
    if (["doc","docx"].includes(ext)) return "fa-file-word";
    return "fa-file";
}

function getIconClass(faIcon) {
    if (faIcon === "fa-file-pdf")  return "pdf-icon";
    if (faIcon === "fa-image")     return "img-icon";
    if (faIcon === "fa-file-word") return "word-icon";
    return "";
}

function formatFileSize(bytes) {
    if (bytes < 1024)         return bytes + " B";
    if (bytes < 1024 * 1024)  return (bytes / 1024).toFixed(1) + " KB";
    return (bytes / (1024 * 1024)).toFixed(1) + " MB";
}

window.updateFileName = function (input, labelId) {
    var label = document.getElementById(labelId);
    if (!label) return;
    label.textContent = (input.files && input.files[0]) ? input.files[0].name : "اضغط لرفع صورة الكارنيه";
};



function initAvatarUpload() {

}

window.previewAvatar = function (input) {
    if (!input.files || !input.files[0]) return;
    var file = input.files[0];

    if (file.size > 5 * 1024 * 1024) {
        alert("حجم الصورة كبير جداً. الحد الأقصى 5 ميجابايت.");
        input.value = "";
        return;
    }

    var reader = new FileReader();
    reader.onload = function (ev) {
        var heroAvatar = document.getElementById("heroAvatar");
        var navAvatar  = document.querySelector(".user-avatar img");
        var src        = ev.target.result;

        if (heroAvatar) { heroAvatar.src = src; heroAvatar.style.display = ""; }
        if (navAvatar)    navAvatar.src  = src;


        showToast("تم تحديث صورة الملف الشخصي");
    };
    reader.readAsDataURL(file);
};



function initFileUploads() {
    var insFile = document.getElementById("insFile");
    if (insFile) {
        insFile.addEventListener("change", function () {
            updateFileName(insFile, "insFileName");
        });
    }
}

function showFieldError(input, message) {
    if (!input) return;
    input.classList.add("input-error");
    var err       = document.createElement("span");
    err.className   = "field-error-msg";
    err.textContent = message;
    err.style.cssText = "color:#e53838; font-size:11px; font-weight:600; display:block; margin-top:4px;";
    input.parentNode.appendChild(err);
}

function clearErrors(form) {
    if (!form) return;
    form.querySelectorAll(".input-error").forEach(function (el) { el.classList.remove("input-error"); });
    form.querySelectorAll(".field-error-msg").forEach(function (el) { el.remove(); });
}


function showToast(message, isError) {
    var toast = document.getElementById("toast");
    var msg   = document.getElementById("toastMsg");
    if (!toast) return;

    if (msg) msg.textContent = message || "تم الحفظ بنجاح";

    toast.style.background = isError ? "#e53838" : "#16a34a";
    toast.classList.add("show");

    setTimeout(function () {
        toast.classList.remove("show");
    }, 3000);
}



document.addEventListener("keydown", function (e) {
    if (e.key !== "Escape") return;
    var dd = document.getElementById("userDropdown");
    if (dd) dd.classList.remove("open");
});

function getAntiForgeryToken() {
    var el = document.querySelector('input[name="__RequestVerificationToken"]');
    return el ? el.value : "";
}