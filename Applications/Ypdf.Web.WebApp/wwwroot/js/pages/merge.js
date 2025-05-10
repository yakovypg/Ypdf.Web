$(document).ready(function () {
    let files = [];

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

        makeFileListSortable();
    }

    function makeFileListSortable() {
        $('#file-list').sortable({
            items: 'li',
            update: function (event, ui) {
                const newOrder = $(this).sortable('toArray', { attribute: 'data-index' });
                files = newOrder.map(index => files[index]);
                updateFileList();
            }
        });
    }

    $('#file-upload').on('change', function (event) {
        files = Array.from(event.target.files);
        updateFileList();
    });

    $('#file-list').on('click', '.remove-file', function () {
        const index = $(this).closest('li').data('index');
        files.splice(index, 1);
        updateFileList();
    });

    $('#merge-button').on('click', function () {
        if (files.length < 2) {
            alert("Upload at least 2 files");
        }
    });
});
