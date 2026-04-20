"use strict";

const tipsData = [

    {
        title: "10 عادات يومية تحمي قلبك من أمراض القلب والأوعية الدموية",
        cat: "heart",
        catLabel: "القلب والأوعية",
        catIcon: "fa-solid fa-heart-pulse",
        content: `
            <p>أمراض القلب والأوعية الدموية هي السبب الأول للوفاة على مستوى العالم، لكن الخبر الجيد أن معظم حالاتها يمكن الوقاية منها بتغييرات بسيطة في نمط الحياة.</p>
            <h4>١. ممارسة النشاط البدني يومياً</h4>
            <p>٣٠ دقيقة على الأقل من التمارين المعتدلة كالمشي السريع أو السباحة تقلل خطر الإصابة بأمراض القلب بنسبة تصل إلى ٣٥٪.</p>
            <h4>٢. الإقلاع عن التدخين فوراً</h4>
            <p>التدخين يُضاعف خطر الإصابة بالنوبة القلبية. بعد عام من الإقلاع، ينخفض الخطر بنسبة ٥٠٪ تقريباً.</p>
            <h4>٣. تناول غذاء صحي للقلب</h4>
            <ul>
                <li>تناول الأسماك الدهنية مرتين أسبوعياً (أوميغا-3)</li>
                <li>أكثر من الخضروات والفواكه والحبوب الكاملة</li>
                <li>قلّل من الملح والسكريات والدهون المشبعة</li>
            </ul>
            <h4>٤. إدارة ضغط الدم والكوليسترول</h4>
            <p>راقب أرقامك باستمرار. ضغط الدم يجب أن يكون أقل من 120/80 ملم زئبق. الكوليسترول الكلي أقل من 200 ملغ/ديسيلتر.</p>
            <div class="tip-warn">
                <i class="fa-solid fa-triangle-exclamation"></i>
                <strong>تنبيه:</strong> إذا شعرت بألم في الصدر أو ضيق في التنفس، توجّه فوراً لأقرب طوارئ.
            </div>
            <h4>٥. النوم الكافي</h4>
            <p>قلة النوم (أقل من 6 ساعات) مرتبطة بزيادة خطر النوبة القلبية. احرص على 7-9 ساعات يومياً.</p>
        `
    },

    {
        title: "فوائد شرب الماء على الريق وكيف يُحسّن صحتك",
        cat: "nutrition",
        catLabel: "التغذية",
        catIcon: "fa-solid fa-apple-whole",
        content: `
            <p>شرب الماء فور الاستيقاظ من النوم هو عادة موثوقة علمياً لها فوائد صحية متعددة تبدأ من أول لحظات يومك.</p>
            <h4>لماذا الريق تحديداً؟</h4>
            <p>أثناء النوم، يمر الجسم بفترة صيام تتراوح بين 6-8 ساعات. شرب الماء فور الاستيقاظ يُعيد ترطيب الخلايا ويُنشّط الدورة الدموية.</p>
            <h4>الفوائد الموثقة</h4>
            <ul>
                <li>تنشيط الجهاز الهضمي وتسريع الأيض بنسبة تصل لـ ٣٠٪</li>
                <li>طرد السموم والفضلات المتراكمة ليلاً عبر الكلى</li>
                <li>تحسين تركيز الدماغ (الجفاف الخفيف يقلل الأداء المعرفي)</li>
                <li>تقليل الشعور بالجوع الزائف الصباحي</li>
            </ul>
            <h4>الكمية المثالية</h4>
            <p>كوب أو كوبان (400-500 مل) من الماء الفاتر أو في درجة حرارة الغرفة. يمكن إضافة عصير نصف ليمونة لتعزيز الفوائد.</p>
            <div class="tip-warn">
                <i class="fa-solid fa-circle-info"></i>
                <strong>ملاحظة:</strong> المقصود بالريق هو قبل أي طعام أو مشروب آخر، وليس بالضرورة قبل الصلاة أو العبادات.
            </div>
        `
    },

    {
        title: "كيف تتحكم في التوتر والقلق بأساليب علمية مجربة",
        cat: "mental",
        catLabel: "الصحة النفسية",
        catIcon: "fa-solid fa-brain",
        content: `
            <p>التوتر المزمن ليس مجرد شعور غير مريح، إنه يؤثر فعلياً على قلبك وجهازك المناعي وأداء دماغك.</p>
            <h4>تقنية التنفس ٤-٧-٨</h4>
            <p>استنشق لـ٤ ثوانٍ، احبس النفس ٧ ثوانٍ، أخرجه ٨ ثوانٍ. تُهدّئ هذا الجهاز العصبي اللاإرادي خلال دقيقتين.</p>
            <h4>التمرين الجسدي كمضاد طبيعي للتوتر</h4>
            <p>٢٠ دقيقة من التمارين تُفرز الإندورفينات وتقلل الكورتيزول (هرمون التوتر) لساعات.</p>
            <h4>تقليل الكافيين والسكر</h4>
            <p>كلاهما يُحفّز إفراز الأدرينالين ويُهيّئ الجسم لحالة "القتال أو الهرب"، مما يُفاقم القلق.</p>
            <ul>
                <li>التأمل اليومي لـ10 دقائق</li>
                <li>تدوين المشاعر في دفتر يومي</li>
                <li>الحد من استخدام وسائل التواصل الاجتماعي مساءً</li>
                <li>التواصل مع أشخاص داعمين</li>
            </ul>
            <div class="tip-warn">
                <i class="fa-solid fa-triangle-exclamation"></i>
                إذا كان القلق يؤثر على حياتك اليومية لأكثر من أسبوعين، استشر طبيباً متخصصاً.
            </div>
        `
    },

    {
        title: "الوقاية من هشاشة العظام: الكالسيوم وفيتامين D",
        cat: "bones",
        catLabel: "العظام والمفاصل",
        catIcon: "fa-solid fa-bone",
        content: `
            <p>هشاشة العظام تُصيب واحدة من كل ثلاث نساء وواحداً من كل خمسة رجال فوق سن الخمسين. الوقاية تبدأ في مرحلة الشباب.</p>
            <h4>مصادر الكالسيوم الغذائية</h4>
            <ul>
                <li>منتجات الألبان: الحليب، الجبن، الزبادي</li>
                <li>الخضروات الورقية: البروكلي، الكرنب</li>
                <li>السردين والسلمون المحفوظ بالعظام</li>
                <li>التوفو المدعّم بالكالسيوم</li>
            </ul>
            <h4>فيتامين D: الشريك الأساسي</h4>
            <p>بدون فيتامين D لا يستطيع جسمك امتصاص الكالسيوم. أفضل مصدر هو التعرض للشمس ١٥-٢٠ دقيقة يومياً.</p>
            <h4>التمارين التي تبني العظام</h4>
            <p>تمارين حمل الوزن كالمشي والجري وتمارين المقاومة تحفّز بناء الكثافة العظمية.</p>
            <div class="tip-warn">
                <i class="fa-solid fa-circle-info"></i>
                النساء بعد سن اليأس والرجال فوق ٧٠ سنة يحتاجون فحص كثافة العظام (DEXA scan) بشكل دوري.
            </div>
        `
    },

    {
        title: "جدول التطعيمات الأساسية للأطفال وأهميته",
        cat: "kids",
        catLabel: "صحة الأطفال",
        catIcon: "fa-solid fa-baby",
        author: "د. نورهان سعيد — طبيبة أطفال",
        date: "٢٥ مارس ٢٠٢٦",
        read: "٥ دقائق",
        content: `
            <p>التطعيمات هي أنجح أدوات الطب الوقائي في التاريخ. بفضلها، اختفت أمراض كانت تودي بحياة الملايين سنوياً.</p>
            <h4>التطعيمات الأساسية خلال السنة الأولى</h4>
            <ul>
                <li><strong>عند الولادة:</strong> التهاب الكبد ب (BCG) ولقاح السل</li>
                <li><strong>شهرين:</strong> السداسي والروتا</li>
                <li><strong>٤ أشهر:</strong> السداسي الجرعة الثانية</li>
                <li><strong>٦ أشهر:</strong> السداسي الثالث والإنفلونزا</li>
                <li><strong>١٢ شهراً:</strong> الحصبة والنكاف والحصبة الألمانية (MMR)</li>
            </ul>
            <h4>إذا فاتك موعد تطعيم</h4>
            <p>لا داعي للقلق. التطعيمات يمكن إعطاؤها لاحقاً وفق جداول اللحاق التي يحددها الطبيب.</p>
            <div class="tip-warn">
                <i class="fa-solid fa-triangle-exclamation"></i>
                التطعيمات آمنة جداً. الحمى الخفيفة بعدها طبيعية وتدل على استجابة الجهاز المناعي. لا تتردد في استشارة طبيب الأطفال.
            </div>
        `
    },

    {
        title: "السيطرة على سكر الدم: نظام غذائي ونمط حياة",
        cat: "diabetes",
        catLabel: "السكري والغدد",
        catIcon: "fa-solid fa-syringe",
        content: `
            <p>السكري من النوع الثاني ليس حكماً نهائياً — بتغيير نمط الحياة يمكن السيطرة عليه وأحياناً عكسه في المراحل المبكرة.</p>
            <h4>الأطعمة المفيدة لمرضى السكري</h4>
            <ul>
                <li>الخضروات غير النشوية: خيار، طماطم، سبانخ</li>
                <li>البقوليات: عدس، فاصوليا، حمص</li>
                <li>الحبوب الكاملة بدلاً من المكررة</li>
                <li>الأسماك والدواجن بدلاً من اللحوم الحمراء</li>
            </ul>
            <h4>مؤشر جلايسيمي (GI) منخفض</h4>
            <p>اختر أطعمة بمؤشر جلايسيمي منخفض (أقل من 55) لتجنب ارتفاع مفاجئ في السكر.</p>
            <h4>النشاط البدني وسكر الدم</h4>
            <p>العضلات تمتص الجلوكوز أثناء التمرين مباشرة دون الحاجة للإنسولين. ٣٠ دقيقة مشي بعد الأكل تقلل ارتفاع السكر بشكل ملحوظ.</p>
            <div class="tip-warn">
                <i class="fa-solid fa-triangle-exclamation"></i>
                لا تُغيّر جرعات أدوية السكري بدون استشارة طبيبك، حتى لو شعرت بتحسن ملحوظ.
            </div>
        `
    },

    {
        title: "حماية بشرتك من الشمس: دليل واقي الشمس الصحيح",
        cat: "skin",
        catLabel: "الجلد والشعر",
        catIcon: "fa-solid fa-sun",
        content: `
            <p>التعرض للشمس بدون حماية هو السبب الأول لشيخوخة الجلد المبكرة وسرطان الجلد، وكلاهما يمكن تجنبهما.</p>
            <h4>كيف تختار SPF المناسب</h4>
            <ul>
                <li><strong>SPF 30:</strong> كافٍ للاستخدام اليومي في الأماكن المغلقة</li>
                <li><strong>SPF 50:</strong> للأنشطة الخارجية والتعرض المطوّل</li>
                <li><strong>SPF 50+:</strong> للشواطئ والمرتفعات والجلود الحساسة</li>
            </ul>
            <h4>طريقة الوضع الصحيحة</h4>
            <p>ضع واقي الشمس قبل ٢٠ دقيقة من الخروج. كمية كافية = ملعقة صغيرة للوجه. أعد الوضع كل ساعتين عند التعرق أو السباحة.</p>
            <h4>الأماكن التي ننساها</h4>
            <ul>
                <li>الرقبة والأذنان والشفاه</li>
                <li>ظهر اليدين والقدمين</li>
                <li>فروة الرأس (استخدم رش الشعر)</li>
            </ul>
            <div class="tip-warn">
                <i class="fa-solid fa-circle-info"></i>
                حتى في الأيام الغائمة، ٨٠٪ من الأشعة فوق البنفسجية تخترق السحب. واقي الشمس ضروري طوال السنة.
            </div>
        `
    },

    {
        title: "ضغط الدم المرتفع: الأعراض الصامتة وطرق السيطرة",
        cat: "heart",
        catLabel: "القلب والأوعية",
        catIcon: "fa-solid fa-heart-pulse",
        content: `
            <p>نحو مليار شخص في العالم يعانون من ارتفاع ضغط الدم، ونصفهم لا يعلمون بذلك.</p>
            <h4>الأرقام الطبيعية</h4>
            <ul>
                <li><strong>طبيعي:</strong> أقل من 120/80 ملم زئبق</li>
                <li><strong>مرتفع:</strong> 130-139 / 80-89</li>
                <li><strong>ارتفاع حاد:</strong> 140/90 فأكثر</li>
            </ul>
            <h4>السيطرة بدون دواء (في الحالات الأولية)</h4>
            <ul>
                <li>تقليل الملح لأقل من 5 جرام يومياً</li>
                <li>فقدان ٥٪ من وزن الجسم يقلل الضغط بمقدار 5 نقاط</li>
                <li>التوقف عن التدخين والكحول</li>
                <li>ممارسة الرياضة ٥ أيام أسبوعياً</li>
                <li>نظام DASH الغذائي</li>
            </ul>
            <div class="tip-warn">
                <i class="fa-solid fa-triangle-exclamation"></i>
                إذا تجاوز الضغط 180/120 فجأة مع صداع شديد أو ضيق تنفس، هذه حالة طوارئ — اذهب للمستشفى فوراً.
            </div>
        `
    },

    {
        title: "الألياف الغذائية: سر الهضم السليم وخفض الكوليسترول",
        cat: "nutrition",
        catLabel: "التغذية",
        catIcon: "fa-solid fa-apple-whole",
        content: `
            <p>الألياف الغذائية ليست مجرد "مُسهّل طبيعي" — هي غذاء البكتيريا النافعة في أمعائك وحارسة مستوى السكر والكوليسترول.</p>
            <h4>نوعا الألياف ووظيفتهما</h4>
            <ul>
                <li><strong>الألياف القابلة للذوبان:</strong> تخفض الكوليسترول وتُبطئ امتصاص السكر (الشوفان، التفاح، الفاصوليا)</li>
                <li><strong>الألياف غير القابلة للذوبان:</strong> تسرّع الحركة المعوية وتمنع الإمساك (النخالة، الخضروات، الحبوب الكاملة)</li>
            </ul>
            <h4>كمية الألياف اليومية الموصى بها</h4>
            <p>٢٥-٣٨ جرام يومياً للبالغين. معظم الناس لا يحصلون على نصف هذه الكمية.</p>
            <h4>كيف تزيد استهلاك الألياف تدريجياً</h4>
            <ul>
                <li>استبدل الخبز الأبيض بخبز القمح الكامل</li>
                <li>أضف حبة فاكهة كاملة لوجبة الإفطار</li>
                <li>تناول حبة من المكسرات كوجبة خفيفة</li>
                <li>أضف الفاصوليا أو العدس لحساء أو سلطة</li>
            </ul>
        `
    },

    {
        title: "النوم الصحي: كم ساعة تحتاج وكيف تحسّن جودته",
        cat: "mental",
        catLabel: "الصحة النفسية",
        catIcon: "fa-solid fa-brain",
        content: `
            <p>النوم ليس ترفاً — هو حاجة بيولوجية أساسية كالطعام والماء. ثلث الأمراض المزمنة مرتبط بقلة النوم أو سوء جودته.</p>
            <h4>الاحتياج حسب العمر</h4>
            <ul>
                <li>الرضع (٠-١٢ شهراً): ١٢-١٦ ساعة</li>
                <li>الأطفال (٦-١٢ سنة): ٩-١٢ ساعة</li>
                <li>المراهقون: ٨-١٠ ساعات</li>
                <li>البالغون: ٧-٩ ساعات</li>
                <li>كبار السن: ٧-٨ ساعات</li>
            </ul>
            <h4>قواعد "نظافة النوم"</h4>
            <ul>
                <li>نم واستيقظ في نفس الوقت يومياً (حتى في الإجازات)</li>
                <li>أبعد الشاشات ٦٠ دقيقة قبل النوم (الضوء الأزرق يقلل الميلاتونين)</li>
                <li>الغرفة مظلمة وباردة (١٨-٢٠ درجة مئوية مثالية)</li>
                <li>تجنب الكافيين بعد الظهر</li>
                <li>لا قيلولة بعد الساعة ٣ عصراً</li>
            </ul>
            <div class="tip-warn">
                <i class="fa-solid fa-triangle-exclamation"></i>
                إذا كنت تشخر بشدة أو تستيقظ متعباً رغم النوم الكافي، قد تعاني من توقف التنفس أثناء النوم — استشر طبيباً.
            </div>
        `
    }
];

let currentCat    = "all";
let currentSearch = "";

window.filterTips = function (cat, btn) {
    currentCat = cat;

    document.querySelectorAll(".tips-filter-tab").forEach(t => t.classList.remove("active"));
    if (btn) btn.classList.add("active");

    renderCards();
};

window.searchTips = function (query) {
    currentSearch = query.trim().toLowerCase();
    renderCards();
};

function renderCards() {
    const cards = document.querySelectorAll(".tip-card");
    let visible = 0;

    cards.forEach(card => {
        const cat   = card.dataset.cat;
        const title = card.querySelector("h3").textContent.toLowerCase();
        const body  = card.querySelector("p").textContent.toLowerCase();

        const catMatch    = currentCat === "all" || cat === currentCat;
        const searchMatch = !currentSearch || title.includes(currentSearch) || body.includes(currentSearch);

        if (catMatch && searchMatch) {
            card.style.display = "flex";
            visible++;
        } else {
            card.style.display = "none";
        }
    });

    const featured = document.getElementById("featuredSection");
    if (featured) {
        featured.style.display = (currentCat === "all" || currentCat === "heart") && !currentSearch ? "block" : "none";
    }

    const countEl = document.getElementById("tipsCount");
    if (countEl) countEl.textContent = `عرض ${visible} نصيحة`;

    const empty = document.getElementById("tipsEmpty");
    if (empty) {
        empty.classList.toggle("show", visible === 0);
    }

    const loadMore = document.getElementById("loadMoreBtn");
    if (loadMore) loadMore.style.display = visible >= cards.length ? "none" : "inline-flex";
}


window.openModal = function (index) {
    const tip = tipsData[index];
    if (!tip) return;

    const catChipClass = tip.cat;
    document.getElementById("modalCategory").innerHTML =
        `<div class="tip-category-chip ${catChipClass}"><i class="${tip.catIcon}"></i> ${tip.catLabel}</div>`;

    document.getElementById("modalTitle").textContent = tip.title;

    document.getElementById("modalContent").innerHTML = tip.content;

    const modal = document.getElementById("tipModal");
    modal.classList.add("open");
    document.body.style.overflow = "hidden";
};

window.closeModal = function () {
    document.getElementById("tipModal").classList.remove("open");
    document.body.style.overflow = "";
};

window.closeModalOutside = function (e) {
    if (e.target === document.getElementById("tipModal")) {
        closeModal();
    }
};

document.addEventListener("keydown", function (e) {
    if (e.key === "Escape") closeModal();
});

document.addEventListener("DOMContentLoaded", function () {
    renderCards();
});
