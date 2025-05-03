$(document).ready(function () {
    const recordsPerHistoryPage = 9;
    let currentHistoryPage = 1;

    const testOperations = [
        { date: "2023-10-01", operation: "Merge", status: "Completed" },
        { date: "2023-09-25", operation: "Split", status: "Completed" },
        { date: "2023-09-20", operation: "Compress", status: "Failed" },
        { date: "2023-09-15", operation: "Merge", status: "Completed" },
        { date: "2023-09-10", operation: "Merge", status: "Completed" },
        { date: "2023-09-05", operation: "Split", status: "Failed" },
        { date: "2023-09-01", operation: "Images to PDF", status: "Completed" },
        { date: "2023-08-25", operation: "Split", status: "Completed" },
        { date: "2023-08-24", operation: "Split", status: "Completed" },
        { date: "2023-08-20", operation: "Compress", status: "Completed" },
        { date: "2023-08-19", operation: "Merge", status: "Completed" },
        { date: "2023-08-17", operation: "Merge", status: "Completed" },
        { date: "2023-08-17", operation: "Compress", status: "Completed" },
        { date: "2023-08-16", operation: "Compress", status: "Completed" },
        { date: "2023-08-15", operation: "Images to PDF", status: "Completed" },
        { date: "2023-08-14", operation: "Split", status: "Completed" },
        { date: "2023-08-14", operation: "Images to PDF", status: "Completed" },
        { date: "2022-10-01", operation: "Merge", status: "Completed" },
        { date: "2022-09-25", operation: "Split", status: "Completed" },
        { date: "2022-09-20", operation: "Compress", status: "Failed" },
        { date: "2022-09-15", operation: "Merge", status: "Completed" },
        { date: "2022-09-10", operation: "Merge", status: "Completed" },
        { date: "2022-09-05", operation: "Split", status: "Failed" },
        { date: "2022-09-01", operation: "Images to PDF", status: "Completed" },
        { date: "2022-08-25", operation: "Split", status: "Completed" },
        { date: "2022-08-24", operation: "Split", status: "Completed" },
        { date: "2022-08-20", operation: "Compress", status: "Completed" },
        { date: "2022-08-19", operation: "Merge", status: "Completed" },
        { date: "2022-08-17", operation: "Merge", status: "Completed" },
        { date: "2022-08-17", operation: "Compress", status: "Completed" },
        { date: "2022-08-16", operation: "Compress", status: "Completed" },
        { date: "2022-08-15", operation: "Images to PDF", status: "Completed" },
        { date: "2022-08-14", operation: "Split", status: "Completed" },
        { date: "2022-08-14", operation: "Images to PDF", status: "Completed" },
    ];

    function displayHistoryRecords(pageNumber, operations, recordsPerPage) {
        const startIndex = (pageNumber - 1) * recordsPerPage;
        const endIndex = startIndex + recordsPerPage;
        const paginatedOperations = operations.slice(startIndex, endIndex);

        $('#operationTable tbody').empty();

        paginatedOperations.forEach(op => {
            $('#operationTable tbody').append(`
                <tr>
                    <td>${op.date}</td>
                    <td>${op.operation}</td>
                    <td>${op.status}</td>
                </tr>
            `);
        });

        $('#page-info').text(`Page ${pageNumber}`);

        $('#prev-button').prop('disabled', pageNumber === 1);
        $('#next-button').prop('disabled', endIndex >= operations.length);
    }

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

    $('#prev-button').on('click', function () {
        if (currentHistoryPage > 1) {
            currentHistoryPage--;
            displayHistoryRecords(currentHistoryPage, testOperations, recordsPerHistoryPage);
        }
    });

    $('#next-button').on('click', function () {
        if ((currentHistoryPage * recordsPerHistoryPage) < testOperations.length) {
            currentHistoryPage++;
            displayHistoryRecords(currentHistoryPage, testOperations, recordsPerHistoryPage);
        }
    });

    displayHistoryRecords(currentHistoryPage, testOperations, recordsPerHistoryPage);
});
