// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var accountDetaislModalAccountId;

$('.js-search-customer').on('click',
    (event) => {
        let firstName = $('.js-search-by-firstname').val();
        let lastName = $('.js-search-by-lastname').val();
        let vatNumber = $('.js-search-by-vatnumber').val();

        var data = {
            firstName: firstName,
            lastName: lastName,
            vatNumber: vatNumber
        };

        //debugger;
        // Make the call to the endpoint
        //debugger;
        var result = $.ajax({
            url: '/customer/search',
            method: 'GET',
            //contentType: 'application/json',
            data: data
        }).done((response) => {
            // Refresh table data
            //debugger;
            console.log('OK');
            $('.js-customers-list tbody').empty();
            for (let i = 0; i < response.length; i++) {
                //debugger;
                $('.js-customers-list tbody').append(
                    `<tr id="${response[i].customerId}">
                        <td>
                            <a href="/customer/${response[i].customerId}">
                                ${response[i].vatNumber}
                            </a>
                        </td>
                        <td>${response[i].firstname}</td>
                        <td>${response[i].lastname}</td>
                     </tr>`);
            }
        });
    });


$('.js-update-customer').on('click',
    (event) => {
        /*debugger;*/
        let firstName = $('.js-first-name').val();
        let lastName = $('.js-last-name').val();
        let customerId = $('.js-customer-id').val();
        let vatNumber = $('.js-vat-number').val();

        console.log(`${firstName} ${lastName}`);

        let data = JSON.stringify({
            firstName: firstName,
            lastName: lastName,
            vatNumber: vatNumber
        });

        $('.js-result').html(' ');
        $('.js-update-customer').attr('disabled', true);
        // ajax call
        let result = $.ajax({
            url: `/customer/${customerId}`,
            method: 'PUT',
            contentType: 'application/json',
            data: data
        }).done(response => {
            // success
            console.log('Update was successful');
            $('.js-result').show();
            $('.js-result')
                .html('<a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a><strong>Update was successful</strong>')
                .addClass('alert alert-success alert-dismissible');
        }).fail(failure => {
            // fail
            console.log('Update failed');
            $('.js-result')
                .html('<a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a><strong>Update failed</strong>')
                .addClass('alert alert-danger alert-dismissible');
        }).always(() => {
            $('.js-update-customer').attr('disabled', false);
        });
    });

$('.js-customers-list tbody tr').on('click',
    (event) => {
        console.log($(event.currentTarget).attr('id'));
    });

$(".accounts-table tbody tr").on('click',
    (event) => {
        let accountId = $(event.currentTarget).attr('id');
        console.log(`${accountId}`);

        var data = {
            accountId: accountId
        };

        $.ajax({
            url: '/account/search',
            method: 'GET',
            data: data
        }).done((response) => {
            //debugger;
            console.log('OK');
            console.log(response);
            $('.modal-body').empty();
            // Add response in Modal body
            //`<div><table>
                //    <tr><td>IBAN: ${response.accountId}</td></tr>
                //    <tr><td>Description: ${response.description}</td></tr>
                //    <tr><td>Currency: ${response.currencyCode}</td></tr>
                //    <tr><td>Balance: ${response.balance}</td></tr>
                //    <tr><td>Created: ${response.auditInfo.created}</td></tr>
                //    <tr><td>Updated: ${response.auditInfo.updated}</td></tr>
                //    <tr><td>State: ${response.state}</td></tr>
                //    </table></div>
                    //`<div class="dropdown-menu">
                    //    <a class="dropdown-item" href="#">Regular link</a>
                    //    <a class="dropdown-item active" href="#">Active link</a>
                    //    <a class="dropdown-item" href="#">Another link</a>
                    //</div>`);

            accountDetaislModalAccountId = response.accountId;

            $('.modal-body').html(
                `<div class="container-fluid">
                    <div class="row">
                        <div class="col-md-4">IBAN:</div>
                        <div class="col-md-4 ms-auto js-account-id">${response.accountId}</div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">Description:</div>
                        <div class="col-md-4 ms-auto">${response.description}</div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">Currency:</div>
                        <div class="col-md-4 ms-auto">${response.currencyCode}</div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">Balance:</div>
                        <div class="col-md-4 ms-auto">${response.balance}</div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">State:</div>
                        <div class="col-md-4 ms-auto">${response.state}</div>
                    </div>
                    <br>
                    <div class="row form-group">
                        <div class="col-md-4">
                            <label for="sel1">Change State to:</label>
                        </div>
                        <div class="col-md-4 ms-auto">
                            <select class="form-control js-state-description" id="sel1">
                                <option>Active</option>
                                <option>Inactive</option>
                                <option>Suspended</option>
                            </select>
                        </div>
                    </div> 
                 </div>`);
                    
            //<br>
            //    <div class="row">
            //        <div class="col-md-4">Created:</div>
            //        <div class="col-md-4 ms-auto">${response.autditInfo.created}</div>
            //    </div>
            //    <br>
            //        <div class="row">
            //            <div class="col-md-4">Updated:</div>
            //            <div class="col-md-4 ms-auto">${response.auditInfo.updated}</div>
            //        </div>
                    
                 //</div>`);

            // Display Modal
            $('#accountModal').modal('show');
        }).fail((response) => {
            console.log('Not OK');
        });
    });

$('.js-update-account').on('click',
    (event) => {
        /*debugger;*/
        let stateDescription = $('.js-state-description').val();
        let accountId = $('.modal-body .js-account-id').val();
        accountId = accountDetaislModalAccountId;

        console.log(`${stateDescription} ${accountId}`);

        let data = JSON.stringify({
            stateDescription: stateDescription
        });

        $('.js-update-account').attr('disabled', true);
        // ajax call
        let result = $.ajax({
            url: `/account/${accountId}`,
            method: 'PUT',
            contentType: 'application/json',
            data: data
        }).done(response => {
            // success
            console.log('Update was successful');

            sleep(2000).then(() => {
                $('#accountModal').modal('toggle');
            });

            //$('.js-result')
            //    .html('<a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a><strong>Update was successful</strong>')
            //    .addClass('alert alert-success alert-dismissible');
        }).fail(failure => {
            // fail
            console.log('Update failed');
            //$('.js-result')
            //    .html('<a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a><strong>Update failed</strong>')
            //    .addClass('alert alert-danger alert-dismissible');
        }).always(() => {
            $('.js-update-account').attr('disabled', false);
        });
    });

function sleep(time) {
    return new Promise((resolve) => setTimeout(resolve, time));
}