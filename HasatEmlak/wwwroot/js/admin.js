// HasatEmlak Admin Panel JavaScript

$(document).ready(function () {
    initializeAdmin();
});

function initializeAdmin() {
    // DataTables initialization
    if ($.fn.DataTable) {
        $('.admin-datatable').DataTable({
            "language": {
                "url": "//cdn.datatables.net/plug-ins/1.10.24/i18n/Turkish.json"
            },
            "pageLength": 25,
            "order": [[0, "desc"]],
            "columnDefs": [
                { "orderable": false, "targets": [-1] } // Last column (actions) not sortable
            ]
        });
    }

    // Initialize tooltips
    if ($.fn.tooltip) {
        $('[data-bs-toggle="tooltip"]').tooltip();
    }

    // Initialize popovers
    if ($.fn.popover) {
        $('[data-bs-toggle="popover"]').popover();
    }

    // Auto-hide alerts
    setTimeout(function () {
        $('.alert').fadeOut();
    }, 5000);

    // Sidebar active link
    highlightActiveLink();

    // Image upload handlers
    initializeImageUpload();

    // Form validation
    initializeFormValidation();
}

// Sidebar navigation
function highlightActiveLink() {
    var currentPath = window.location.pathname;
    $('.sidebar .nav-link').each(function () {
        var linkPath = $(this).attr('href');
        if (currentPath.indexOf(linkPath) === 0 && linkPath !== '/') {
            $(this).addClass('active');
        }
    });
}

// Image Upload Functions
function initializeImageUpload() {
    // Drag and drop
    $('.image-upload-container').on('dragover', function (e) {
        e.preventDefault();
        $(this).addClass('dragover');
    });

    $('.image-upload-container').on('dragleave', function (e) {
        e.preventDefault();
        $(this).removeClass('dragover');
    });

    $('.image-upload-container').on('drop', function (e) {
        e.preventDefault();
        $(this).removeClass('dragover');

        var files = e.originalEvent.dataTransfer.files;
        handleImageFiles(files);
    });

    // File input change
    $('input[type="file"][accept*="image"]').on('change', function () {
        handleImageFiles(this.files);
    });
}

function handleImageFiles(files) {
    var validTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];
    var maxSize = 5 * 1024 * 1024; // 5MB

    for (var i = 0; i < files.length; i++) {
        var file = files[i];

        if (!validTypes.includes(file.type)) {
            showAlert('Geçersiz dosya formatı: ' + file.name, 'warning');
            continue;
        }

        if (file.size > maxSize) {
            showAlert('Dosya çok büyük: ' + file.name + ' (Max: 5MB)', 'warning');
            continue;
        }

        // Preview image
        var reader = new FileReader();
        reader.onload = function (e) {
            addImagePreview(e.target.result, file.name);
        };
        reader.readAsDataURL(file);
    }
}

function addImagePreview(src, filename) {
    var previewHtml = `
        <div class="uploaded-image">
            <img src="${src}" alt="${filename}">
            <button type="button" class="remove-image" onclick="removeImagePreview(this)">
                <i class="fas fa-times"></i>
            </button>
        </div>
    `;
    $('.uploaded-images').append(previewHtml);
}

function removeImagePreview(button) {
    $(button).closest('.uploaded-image').remove();
}

// Form Validation
function initializeFormValidation() {
    // Custom validation messages
    $('form').on('submit', function (e) {
        var isValid = true;

        // Required field validation
        $(this).find('[required]').each(function () {
            if (!$(this).val()) {
                $(this).addClass('is-invalid');
                isValid = false;
            } else {
                $(this).removeClass('is-invalid').addClass('is-valid');
            }
        });

        // Email validation
        $(this).find('input[type="email"]').each(function () {
            var email = $(this).val();
            if (email && !isValidEmail(email)) {
                $(this).addClass('is-invalid');
                isValid = false;
            }
        });

        if (!isValid) {
            e.preventDefault();
            showAlert('Lütfen tüm zorunlu alanları doldurun!', 'danger');
        }
    });

    // Real-time validation
    $('input, select, textarea').on('blur', function () {
        if ($(this).attr('required') && !$(this).val()) {
            $(this).addClass('is-invalid');
        } else {
            $(this).removeClass('is-invalid').addClass('is-valid');
        }
    });
}

// Utility Functions
function isValidEmail(email) {
    var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

function showAlert(message, type = 'info') {
    var alertHtml = `
        <div class="alert alert-${type} alert-dismissible fade show" role="alert">
            <i class="fas fa-${getAlertIcon(type)} me-2"></i>${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;

    // Remove existing alerts
    $('.alert').remove();

    // Add new alert
    $('main').prepend(alertHtml);

    // Auto-hide after 5 seconds
    setTimeout(function () {
        $('.alert').fadeOut();
    }, 5000);
}

function getAlertIcon(type) {
    switch (type) {
        case 'success': return 'check-circle';
        case 'danger': return 'exclamation-circle';
        case 'warning': return 'exclamation-triangle';
        case 'info': return 'info-circle';
        default: return 'info-circle';
    }
}

function confirmAction(message, callback) {
    if (confirm(message)) {
        callback();
    }
}

function showLoadingButton(button, loadingText = 'İşleniyor...') {
    var $btn = $(button);
    $btn.data('original-text', $btn.html());
    $btn.html(`<span class="spinner-border spinner-border-sm me-2"></span>${loadingText}`);
    $btn.prop('disabled', true);
}

function hideLoadingButton(button) {
    var $btn = $(button);
    $btn.html($btn.data('original-text'));
    $btn.prop('disabled', false);
}

// AJAX Functions
function makeAjaxRequest(url, data, method = 'GET', successCallback, errorCallback) {
    $.ajax({
        url: url,
        type: method,
        data: data,
        dataType: 'json',
        success: function (response) {
            if (successCallback) {
                successCallback(response);
            }
        },
        error: function (xhr, status, error) {
            console.error('AJAX Error:', error);
            if (errorCallback) {
                errorCallback(xhr, status, error);
            } else {
                showAlert('Bir hata oluştu. Lütfen tekrar deneyin.', 'danger');
            }
        }
    });
}

// Delete Functions
function deleteItem(id, url, itemName = 'öğe') {
    if (confirm(`Bu ${itemName}'yi silmek istediğinizden emin misiniz?\n\nBu işlem geri alınamaz!`)) {
        $.ajax({
            url: url,
            type: 'POST',
            data: { id: id },
            success: function (response) {
                if (response.success) {
                    showAlert(response.message || `${itemName} başarıyla silindi!`, 'success');
                    // Reload page or remove row
                    setTimeout(function () {
                        location.reload();
                    }, 1500);
                } else {
                    showAlert(response.message || 'Silme işlemi başarısız!', 'danger');
                }
            },
            error: function () {
                showAlert('Bir hata oluştu. Lütfen tekrar deneyin.', 'danger');
            }
        });
    }
}

// Toggle Status Functions
function toggleStatus(id, url, currentStatus) {
    var action = currentStatus ? 'pasif' : 'aktif';
    var message = `Bu öğeyi ${action} yapmak istediğinizden emin misiniz?`;

    if (confirm(message)) {
        $.ajax({
            url: url,
            type: 'POST',
            data: { id: id, status: !currentStatus },
            success: function (response) {
                if (response.success) {
                    showAlert(response.message || `Durum başarıyla ${action} yapıldı!`, 'success');
                    setTimeout(function () {
                        location.reload();
                    }, 1500);
                } else {
                    showAlert(response.message || 'İşlem başarısız!', 'danger');
                }
            },
            error: function () {
                showAlert('Bir hata oluştu. Lütfen tekrar deneyin.', 'danger');
            }
        });
    }
}

// Property specific functions
function toggleFeatured(propertyId, isFeatured) {
    var action = isFeatured ? 'öne çıkarılandan kaldır' : 'öne çıkar';

    if (confirm(`Bu ilanı ${action}mak istediğinizden emin misiniz?`)) {
        $.ajax({
            url: '/Admin/Property/ToggleFeatured',
            type: 'POST',
            data: { id: propertyId, featured: !isFeatured },
            success: function (response) {
                if (response.success) {
                    showAlert(response.message, 'success');
                    setTimeout(function () {
                        location.reload();
                    }, 1500);
                } else {
                    showAlert(response.message || 'İşlem başarısız!', 'danger');
                }
            },
            error: function () {
                showAlert('Bir hata oluştu. Lütfen tekrar deneyin.', 'danger');
            }
        });
    }
}

// Location dependent dropdowns
function initializeLocationDropdowns() {
    $('#citySelect').on('change', function () {
        var cityId = $(this).val();
        loadDistricts(cityId);
    });

    $('#districtSelect').on('change', function () {
        var districtId = $(this).val();
        loadNeighborhoods(districtId);
    });
}

function loadDistricts(cityId) {
    var districtSelect = $('#districtSelect');
    var neighborhoodSelect = $('#neighborhoodSelect');

    districtSelect.html('<option value="">Yükleniyor...</option>');
    neighborhoodSelect.html('<option value="">İlçe seçin</option>');

    if (cityId) {
        $.get('/Property/GetDistricts', { cityId: cityId }, function (data) {
            var options = '<option value="">İlçe Seçin</option>';
            $.each(data, function (index, district) {
                options += `<option value="${district.id}">${district.name}</option>`;
            });
            districtSelect.html(options);
        }).fail(function () {
            districtSelect.html('<option value="">Hata oluştu</option>');
            showAlert('İlçeler yüklenirken hata oluştu!', 'warning');
        });
    } else {
        districtSelect.html('<option value="">İlçe Seçin</option>');
        neighborhoodSelect.html('<option value="">Mahalle Seçin</option>');
    }
}

function loadNeighborhoods(districtId) {
    var neighborhoodSelect = $('#neighborhoodSelect');

    neighborhoodSelect.html('<option value="">Yükleniyor...</option>');

    if (districtId) {
        $.get('/Property/GetNeighborhoods', { districtId: districtId }, function (data) {
            var options = '<option value="">Mahalle Seçin</option>';
            $.each(data, function (index, neighborhood) {
                options += `<option value="${neighborhood.id}">${neighborhood.name}</option>`;
            });
            neighborhoodSelect.html(options);
        }).fail(function () {
            neighborhoodSelect.html('<option value="">Hata oluştu</option>');
            showAlert('Mahalleler yüklenirken hata oluştu!', 'warning');
        });
    } else {
        neighborhoodSelect.html('<option value="">Mahalle Seçin</option>');
    }
}

// Bulk Actions
function initializeBulkActions() {
    // Select all checkbox
    $('#selectAll').on('change', function () {
        $('input[name="selectedIds"]').prop('checked', $(this).is(':checked'));
        updateBulkActionButtons();
    });

    // Individual checkboxes
    $(document).on('change', 'input[name="selectedIds"]', function () {
        updateBulkActionButtons();

        // Update select all checkbox
        var totalCheckboxes = $('input[name="selectedIds"]').length;
        var checkedCheckboxes = $('input[name="selectedIds"]:checked').length;

        $('#selectAll').prop('indeterminate', checkedCheckboxes > 0 && checkedCheckboxes < totalCheckboxes);
        $('#selectAll').prop('checked', checkedCheckboxes === totalCheckboxes);
    });

    // Bulk action buttons
    $('#bulkActivate').on('click', function () {
        executeBulkAction('activate', 'Seçili öğeleri aktif yap');
    });

    $('#bulkDeactivate').on('click', function () {
        executeBulkAction('deactivate', 'Seçili öğeleri pasif yap');
    });

    $('#bulkDelete').on('click', function () {
        executeBulkAction('delete', 'Seçili öğeleri sil');
    });
}

function updateBulkActionButtons() {
    var selectedCount = $('input[name="selectedIds"]:checked').length;
    var $bulkActions = $('.bulk-actions');

    if (selectedCount > 0) {
        $bulkActions.show();
        $bulkActions.find('.selected-count').text(selectedCount);
    } else {
        $bulkActions.hide();
    }
}

function executeBulkAction(action, confirmMessage) {
    var selectedIds = [];
    $('input[name="selectedIds"]:checked').each(function () {
        selectedIds.push($(this).val());
    });

    if (selectedIds.length === 0) {
        showAlert('Lütfen en az bir öğe seçin!', 'warning');
        return;
    }

    if (confirm(`${confirmMessage}?\n\n${selectedIds.length} öğe seçildi.`)) {
        $.ajax({
            url: `/Admin/Property/BulkAction`,
            type: 'POST',
            data: {
                action: action,
                ids: selectedIds
            },
            success: function (response) {
                if (response.success) {
                    showAlert(response.message, 'success');
                    setTimeout(function () {
                        location.reload();
                    }, 1500);
                } else {
                    showAlert(response.message || 'İşlem başarısız!', 'danger');
                }
            },
            error: function () {
                showAlert('Bir hata oluştu. Lütfen tekrar deneyin.', 'danger');
            }
        });
    }
}

// Export Functions
function exportData(format, url) {
    showAlert('Dışa aktarma başlatılıyor...', 'info');

    // Create a hidden form to submit the export request
    var form = $('<form>', {
        'method': 'POST',
        'action': url
    });

    form.append($('<input>', {
        'type': 'hidden',
        'name': 'format',
        'value': format
    }));

    // Add CSRF token if available
    var token = $('input[name="__RequestVerificationToken"]').val();
    if (token) {
        form.append($('<input>', {
            'type': 'hidden',
            'name': '__RequestVerificationToken',
            'value': token
        }));
    }

    // Submit form
    form.appendTo('body').submit().remove();
}

// Search and Filter Functions
function initializeSearch() {
    var searchTimeout;

    $('#searchInput').on('input', function () {
        clearTimeout(searchTimeout);
        var query = $(this).val();

        searchTimeout = setTimeout(function () {
            if (query.length >= 2 || query.length === 0) {
                performSearch(query);
            }
        }, 500);
    });
}

function performSearch(query) {
    var currentUrl = new URL(window.location);

    if (query) {
        currentUrl.searchParams.set('search', query);
    } else {
        currentUrl.searchParams.delete('search');
    }

    currentUrl.searchParams.delete('page'); // Reset to first page
    window.location.href = currentUrl.toString();
}

// Initialize functions when document is ready
$(document).ready(function () {
    initializeLocationDropdowns();
    initializeBulkActions();
    initializeSearch();
});