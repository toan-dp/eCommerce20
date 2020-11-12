const CART_DETAIL_CONTROLS = {
    cart: {
        updateCartTotalPrice: function (total) {
            $('#cart_total_price').html(total + 'usd')
        },
        updateCartTotalItem: function (total) {
            $('#cart_total_item').html(total)
        },
        updateCartTotalQuantity: function (total) {
            $('#cart_total_quantity').html(total)
        },
        updateCartItemQuantity: function (cartItemId, action) {
            var quantity = +$(`#cart_item_${cartItemId} .cart_item_quantity`)
                .attr('quantity');
            var unitPrice = +$(`#cart_item_${cartItemId} .cart_item_total`)
                .attr('unit-price');

            switch (action) {
                case "increase":
                    ++quantity;
                    break;
                case "decrease":
                    --quantity;
                    break;
                case "remove":
                    $('#cart_item_' + cartItemId).remove();
                    return;
            }
            $(`#cart_item_${cartItemId} .cart_item_quantity`).attr('quantity', quantity);
            $(`#cart_item_${cartItemId} .cart_item_quantity`).html(quantity);

            debugger;
            $(`#cart_item_${cartItemId} .cart_item_total`).html(quantity * unitPrice + " usd");

        },
        remove: function (cartItemId, productId) {
            var that = this;
            CART_MANAGER.removeFromCart(productId,
                (result) => {
                    that.updateCartTotalItem(result.cartTotalItems);
                    that.updateCartTotalQuantity(result.cartTotalQuantity);
                    that.updateCartItemQuantity(cartItemId, 'remove');
                    that.updateCartTotalPrice(result.cartTotalPrice);
                });
        },
        increase: function (cartItemId, productId) {
            var that = this;
            CART_MANAGER.increaseCartItemQuantity(productId,
                (result) => {
                    that.updateCartTotalItem(result.cartTotalItems);
                    that.updateCartTotalQuantity(result.cartTotalQuantity);
                    that.updateCartItemQuantity(cartItemId, 'increase');
                    that.updateCartTotalPrice(result.cartTotalPrice);
                });
        },
        decrease: function (cartItemId, productId) {
            var that = this;
            CART_MANAGER.decreaseCartItemQuantity(productId,
                (result) => {
                    if (result.cartItemQuantity == 0) {
                        that.remove(cartItemId, productId);
                    }
                    else {
                        that.updateCartTotalItem(result.cartTotalItems);
                        that.updateCartTotalQuantity(result.cartTotalQuantity);
                        that.updateCartItemQuantity(cartItemId, 'decrease');
                        that.updateCartTotalPrice(result.cartTotalPrice);
                    }
                });
        }
    }
};