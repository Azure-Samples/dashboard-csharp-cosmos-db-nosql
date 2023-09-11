window.createChart = (id, config) => {
    var target = document.getElementById(id).getContext('2d');
    Chart.defaults.color = '#ffffffbf';
    config.options = {
        responsive: true,
        plugins: {
            legend: {
                display: false
            }
        }
    };
    new Chart(target, config);
}