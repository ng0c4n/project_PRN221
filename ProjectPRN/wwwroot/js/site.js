$(() => {
    var currentPage = 1;
    var pageSize = 5;

    var connection = new signalR.HubConnectionBuilder().withUrl("/signalrServer").build();

    connection.start();
    LoadPostData(currentPage, pageSize);
    connection.on("LoadProducts", function () {
        LoadPostData(currentPage, pageSize);
    });
});

function LoadPostData(page, pageSize) {
    var str = '';
    $.ajax({
        url: `/Products/GetAllProduct?page=${page}&pageSize=${pageSize}`,
        method: 'GET',
        success: (result) => {
            console.log('Response:', result);
            const tableBody = $('.item-product');

            tableBody.html(
                result.map(
                    (v) => `<div class="col-sm-6 col-md-4 col-lg-3">
                <div class="box rounded">
                    <div>
                        <div class="img-box">
                            <img src="${v.image}" alt="">
                        </div>
                        <div class="detail-box">
                            <h6>
                                ${v.name}
                            </h6>
                            <h6>
                                Price: 
                                <span>
                                   $${v.price}
                                </span>
                            </h6>
                        </div>
                        <div> ${v.description} </div>

                        <div class="new">
                            <span>
                                New
                            </span>
                        </div>
                        <div class="w-100 text-end">
                            <button type="button" class="btn btn-success my-1" data-bs-toggle="modal" data-bs-target="#staticBackdrop${v.id}">
                                Add to cart
                            </button>
                        </div>

                        <!-- Modal -->
                        <div class="modal fade" id="staticBackdrop${v.id}" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="staticBackdropLabel" aria-hidden="true">
                            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header" style="background-color:violet;">
                                        <h5 class="modal-title" id="staticBackdropLabel">Add to card</h5>
                                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                    </div>
                                    <div class="modal-body">
                                       <form id="AddCartForm" method="post" asp-page-handler="AddToCart">
                                            <div class="form-actions no-color">
                                                <p>
                                                    <input id="productId" value="${v.id}" class="form-control" type="text" hidden/>
                                                    
                                                    Product Name:
                                                    <input id="productName" value="${v.name}" class="form-control" type="text" disabled/>
                                                    <img src="${v.image}" alt="">
                                                    Descriptiom:
                                                    <p id="description" class="form-control" type="text">
                                                        ${v.description}
                                                    </p>
                                                    Price Per One:
                                                    <input id="price"  value="${v.price}" class="form-control" type="number" disabled/>
                                                    Quantity:
                                                    <input id="quantity" class="form-control" type="number"/>

                                                    <div class="text-end">
                                                        <input type="submit" value="Submit" class="btn btn-primary m-1" />
                                                    </div>
                                                </p>
                                            </div>
                                        </form>
                                        <div id="reportTable"></div>
                                    </div>
                                    <div class="modal-footer">
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>`
                ).join('')
            );
            updatePaginationUI(page);
        },
        error: (error) => {
            console.log(error);
        }
    });
}

$(document).on('submit', '#AddCartForm', function (event) {
    event.preventDefault();

    const productId = $(this).find('#productId').val();
    const quantity = $(this).find('#quantity').val();

    const cartItem = {
        productId: productId,
        quantity: quantity
    };

    $.ajax({
        url: '/Products/AddToCart',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(cartItem),
        success: function (response) {
            console.log('Thêm vào giỏ hàng thành công');
            alert("Thêm vào giỏ hàng thành công")
        },
        error: function (error) {
            console.log('Lỗi khi thêm vào giỏ hàng:', error);
        }
    });
});

function updatePaginationUI(page) {
    $('#prevPage').prop('disabled', page === 1);
    $('#nextPage').prop('disabled', $('.tableBody tr').length < pageSize);
}

$('#searchButton').click(() => {
    const searchTerm = $('#searchInput').val();
    if (searchTerm != '') {
        SearchPost(searchTerm, currentPage, pageSize);
    }
    else {
        LoadPostData(currentPage, pageSize);
    }
});

$('#prevPage').click(() => {
    if (currentPage > 1) {
        currentPage--;
        LoadPostData(currentPage, pageSize);
    }
});

$('#nextPage').click(() => {
    currentPage++;
    LoadPostData(currentPage, pageSize);
});

function SearchPost(title, page, pageSize) {
    $.ajax({
        url: `/Products/Search/${title}?page=${page}&pageSize=${pageSize}`,
        method: 'GET',
        success: (result) => {
            console.log('Search Result:', result);
            const tableBody = $('.item-product');
            tableBody.html(
                result.map(
                    (v) => `<div class="col-sm-6 col-md-4 col-lg-3">
                <div class="box rounded">
                    <div>
                        <div class="img-box">
                            <img src=${v.image} alt="">
                        </div>
                        <div class="detail-box">
                            <h6>
                                ${v.name}
                            </h6>
                            <h6>
                                Price: 
                                <span>
                                   $${v.price}
                                </span>
                            </h6>
                        </div>
                        <div> ${v.description} </div>

                        <div class="new">
                            <span>
                                New
                            </span>
                        </div>
                        <div class="w-100 text-end">
                            <button type="button" class="btn btn-success my-1" data-bs-toggle="modal" data-bs-target="#staticBackdrop">
                                Add to cart
                            </button>
                        </div>

                        <!-- Modal -->
                        <div class="modal fade" id="staticBackdrop" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="staticBackdropLabel" aria-hidden="true">
                            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header" style="background-color:violet;">
                                        <h5 class="modal-title" id="staticBackdropLabel">Add to card</h5>
                                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                    </div>
                                    <div class="modal-body">
                                        <form id="AddCartForm" method="post" asp-page-handler="AddToCart">
                                            <div class="form-actions no-color">
                                                <p>
                                                    Product Name:
                                                    <input id="productName" value="${v.name}" class="form-control" type="text" disabled/>
                                                    <img src="${v.image}" alt="">
                                                    Descriptiom:
                                                    <input id="description"  value="${v.description}" class="form-control" type="text" disabled/>
                                                    Price Per One:
                                                    <input id="price"  value="${v.price}" class="form-control" type="number" disabled/>
                                                    Quantity:
                                                    <input id="quantity" class="form-control" type="number"/>

                                                    <div class="text-end">
                                                        <input type="submit" value="Submit" class="btn btn-primary m-1" />
                                                    </div>
                                                </p>
                                            </div>
                                        </form>
                                        <div id="reportTable"></div>
                                    </div>
                                    <div class="modal-footer">
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>`
                ).join('')
            );
            updatePaginationUI(page);
        },
        error: (error) => {
            console.log(error);
        }
    });
}

