// تابع برای ایجاد نمودار با Chart.js
window.createChart = function (canvasId, chartType, labels, datasets, options) {
    const ctx = document.getElementById(canvasId).getContext('2d');

    // حذف نمودار قبلی اگر وجود داشته باشد
    if (window.chartInstances && window.chartInstances[canvasId]) {
        window.chartInstances[canvasId].destroy();
    }

    const chart = new Chart(ctx, {
        type: chartType,
        data: {
            labels: labels,
            datasets: datasets
        },
        options: options || {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom'
                }
            }
        }
    });

    // ذخیره نمونه نمودار برای استفاده بعدی
    if (!window.chartInstances) {
        window.chartInstances = {};
    }
    window.chartInstances[canvasId] = chart;
};

// تابع برای حذف نمودار (در صورت نیاز)
window.destroyChart = function (canvasId) {
    if (window.chartInstances && window.chartInstances[canvasId]) {
        window.chartInstances[canvasId].destroy();
        delete window.chartInstances[canvasId];
    }
};