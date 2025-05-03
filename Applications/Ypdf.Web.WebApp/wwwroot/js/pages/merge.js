$(document).ready(function () {
    let files = [];

    // Update the file list display
    function updateFileList() {
        $('#file-list').empty();

        files.forEach((file, index) => {
            const listItem = $(`
                <li class="list-group-item d-flex justify-content-between align-items-center" data-index="${index}">
                    ${file.name}
                    <div>
                        <button class="btn btn-danger btn-sm remove-file">Remove</button>
                    </div>
                </li>
            `);

            $('#file-list').append(listItem);
        });

        makeListSortable();
    }

    // Make the file list sortable
    function makeListSortable() {
        $('#file-list').sortable({
            items: 'li',
            update: function (event, ui) {
                const newOrder = $(this).sortable('toArray', { attribute: 'data-index' });
                files = newOrder.map(index => files[index]);
                updateFileList();
            }
        });
    }

    // Handle file upload
    $('#file-upload').on('change', function (event) {
        const newFiles = Array.from(event.target.files);
        files = files.concat(newFiles);
        updateFileList();

        // Uncomment to reset last uploaded files label text
        // event.target.value = '';
    });

    // Remove file from the list
    $('#file-list').on('click', '.remove-file', function () {
        const index = $(this).closest('li').data('index');
        files.splice(index, 1);
        updateFileList();
    });

    // Handle merge button click
    $('#merge-button').on('click', function () {
        if (files.length < 2) {
            alert("Upload at least 2 files");
        }
    });
});
