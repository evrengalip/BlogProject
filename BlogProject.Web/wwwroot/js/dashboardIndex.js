$(document).ready(function () {
    // Toplam makale sayısını al
    $.ajax({
        type: "GET",
        url: app.Urls.totalArticleCountUrl,
        dataType: "json",
        success: function (data) {
            $("#totalArticleCount").text(data);
        },
        error: function () {
            $("#totalArticleCount").text("Hata");
        }
    });

    // Toplam kategori sayısını al
    $.ajax({
        type: "GET",
        url: app.Urls.totalCategoryCountUrl,
        dataType: "json",
        success: function (data) {
            $("#totalCategoryCount").text(data);
        },
        error: function () {
            $("#totalCategoryCount").text("Hata");
        }
    });

    // Yıllık makale dağılımı için veri al
    $.ajax({
        type: "GET",
        url: app.Urls.yearlyArticlesUrl,
        dataType: "json",
        success: function (data) {
            initializeRevenueChart(data);
        },
        error: function () {
            console.log("Makale dağılımı verisi alınamadı");
        }
    });

    // Makale dağılımı grafiği oluştur
    function initializeRevenueChart(datas) {
        const months = ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran', 'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'];

        let options = {
            series: [{
                name: 'Makale Sayısı',
                data: datas
            }],
            chart: {
                height: 350,
                type: 'bar',
                toolbar: {
                    show: false
                }
            },
            plotOptions: {
                bar: {
                    borderRadius: 4,
                    columnWidth: '30%',
                }
            },
            colors: ['#696cff'],
            dataLabels: {
                enabled: false
            },
            xaxis: {
                categories: months,
                axisBorder: {
                    show: false
                }
            },
            yaxis: {
                labels: {
                    formatter: function (val) {
                        return Math.floor(val);
                    }
                },
                min: 0,
                tickAmount: 4
            },
            tooltip: {
                y: {
                    formatter: function (val) {
                        return val + " Makale";
                    }
                }
            }
        };

        const revenueChart = new ApexCharts(document.querySelector("#customTotalRevenueChart"), options);
        revenueChart.render();
    }
});