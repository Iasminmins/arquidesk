function renderChart(canvasId, data, type, label) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) {
        return;
    }

    const labels = Object.keys(data);
    const values = Object.values(data);

    new Chart(canvas, {
        type,
        data: {
            labels,
            datasets: [{
                label,
                data: values,
                backgroundColor: ["#2563eb", "#14b8a6", "#f59e0b", "#ef4444", "#64748b", "#7c3aed", "#10b981", "#f97316"],
                borderWidth: 0,
                borderRadius: type === "bar" ? 8 : 0,
                maxBarThickness: 42
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            layout: {
                padding: 4
            },
            plugins: {
                legend: {
                    display: type === "doughnut",
                    position: "bottom",
                    labels: {
                        boxWidth: 10,
                        boxHeight: 10,
                        padding: 16,
                        usePointStyle: true
                    }
                }
            },
            scales: type === "bar" ? {
                x: {
                    grid: {
                        display: false
                    },
                    ticks: {
                        color: "#64748b"
                    }
                },
                y: {
                    beginAtZero: true,
                    ticks: {
                        precision: 0,
                        color: "#64748b"
                    },
                    grid: {
                        color: "#eef2f7"
                    }
                }
            } : {}
        }
    });
}

function renderEmptyChart(canvasId, message) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) {
        return;
    }

    const box = canvas.closest(".chart-box");
    if (box) {
        box.innerHTML = `<div class="empty-chart">${message}</div>`;
    }
}

function hasChartData(data) {
    return Object.values(data || {}).some(value => Number(value) > 0);
}

function renderDashboardCharts(data) {
    if (hasChartData(data.status)) {
        renderChart("statusChart", data.status, "doughnut", "Chamados");
    } else {
        renderEmptyChart("statusChart", "Nenhum chamado para exibir");
    }

    if (hasChartData(data.priority)) {
        renderChart("priorityChart", data.priority, "bar", "Chamados");
    } else {
        renderEmptyChart("priorityChart", "Nenhuma prioridade registrada");
    }

    if (hasChartData(data.type)) {
        renderChart("typeChart", data.type, "bar", "Chamados");
    } else {
        renderEmptyChart("typeChart", "Nenhum tipo registrado");
    }
}
