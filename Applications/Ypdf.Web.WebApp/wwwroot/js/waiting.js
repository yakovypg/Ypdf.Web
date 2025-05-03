$(document).ready(function () {
    let seconds = 0;
    const timerElement = $('#timer');
    const resultElement = $('#result');

    let attempts = 0;
    const maxAttempts = 10;

    timerElement.text(seconds);

    const interval = setInterval(function () {
        seconds++;
        timerElement.text(seconds);
    }, 1000);

    function checkOperationStatus() {
        $.get('https://localhost:8082/api/output/check/', function (data) {
            if (data.completed) {
                clearInterval(interval);
                resultElement.html(`
                    <h5>Operation completed successfully!</h5>
                    <a href="${data.downloadLink}" class="btn btn-success mt-3">Download File</a>
                    <button class="btn btn-info mt-3" onclick="copyLink('${data.downloadLink}')">Copy Link</button>
                `);
            } else {
                attempts++;
                if (attempts >= maxAttempts) {
                    clearInterval(interval);
                    resultElement.html(`
                        <h5>Something went wrong!</h5>
                        <p>We are unable to get the result from the server after multiple attempts.</p>
                    `);
                } else {
                    setTimeout(checkOperationStatus, 2000); // Wait 2 seconds before the next request
                }
            }
        }).fail(function () {
            clearInterval(interval);
            resultElement.html(`
                <h5>Error occurred!</h5>
                <p>Unable to reach the server. Please try again later.</p>
            `);
        });
    }

    // Start checking the operation status
    checkOperationStatus();

    // Function to copy the download link to clipboard
    window.copyLink = function (link) {
        navigator.clipboard.writeText(link).then(function () {
            alert('Link copied to clipboard!');
        }, function () {
            alert('Failed to copy the link.');
        });
    };
});
