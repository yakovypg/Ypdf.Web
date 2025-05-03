$(document).ready(function () {
    let totalPages = 0;

    // Validate page ranges
    function validateRanges(ranges) {
        const rangeArray = ranges.split(',').map(r => r.trim());
        const parsedRanges = [];

        for (let range of rangeArray) {
            const match = range.match(/^(\d+)(-(\d+))?$/);
            if (!match) {
                return 'Invalid range format: ' + range;
            }
            const start = parseInt(match[1]);
            const end = match[3] ? parseInt(match[3]) : start;

            if (start < 1 || end < 1) {
                return 'Page numbers must be positive.';
            }
            if (start > end) {
                return 'Start of range cannot be greater than end: ' + range;
            }
            if (end > totalPages) {
                return 'Page number exceeds total pages (' + totalPages + '): ' + range;
            }

            for (let existingRange of parsedRanges) {
                if ((start >= existingRange.start && start <= existingRange.end) ||
                    (end >= existingRange.start && end <= existingRange.end) ||
                    (start <= existingRange.start && end >= existingRange.end)) {
                    return 'Overlapping ranges detected: ' + range + ' overlaps with ' + existingRange.start + '-' + existingRange.end;
                }
            }

            parsedRanges.push({ start: start, end: end });
        }

        // No errors
        return null;
    }

    // Handle file upload
    $('#file-upload').on('change', function (event) {
        const file = event.target.files[0];
        if (file) {
            const fileReader = new FileReader();
            fileReader.onload = function () {
                const typedarray = new Uint8Array(this.result);
                pdfjsLib.getDocument(typedarray).promise.then(function (pdf) {
                    totalPages = pdf.numPages;
                    $('#error-message').text('');
                }).catch(function (error) {
                    $('#error-message').text('Error loading PDF: ' + error.message);
                });
            };
            fileReader.readAsArrayBuffer(file);
        }
    });

    // Handle split button click
    $('#split-button').on('click', function () {
        const ranges = $('#page-ranges').val().trim();
        const error = validateRanges(ranges);
        if (error) {
            $('#error-message').text(error);
        } else {
            $('#error-message').text('');
            alert('PDF will be split for ranges: ' + ranges);
        }
    });
});
