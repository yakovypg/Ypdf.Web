$(document).ready(function () {
    document.getElementById('subscription-toggle').addEventListener('change', function() {
        const isChecked = this.checked;
        const toggleLabel = document.getElementById('toggle-label');

        if (isChecked) {
            toggleLabel.textContent = 'Standard';
            document.querySelectorAll('.trial-benefit').forEach(el => el.style.display = 'none');
            document.querySelectorAll('.standard-benefit').forEach(el => el.style.display = 'block');
        } else {
            toggleLabel.textContent = 'Trial';
            document.querySelectorAll('.trial-benefit').forEach(el => el.style.display = 'block');
            document.querySelectorAll('.standard-benefit').forEach(el => el.style.display = 'none');
        }
    });
});
