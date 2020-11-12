const CART_MANAGER = {
    updateCartCounterRequest: function () {
        $.post("/ShoppingCart/CountCartItems", {
        }, (result) => {
            if (result.isSuccess) {
                $('#shop_cart #cart_counter').html(result.total)
            }
        });
    },
    addToCart: function (productId, callBack) {
        var that = this;
        $.post("/ShoppingCart/AddToCart", {
            productId
        }, (result) => {
            debugger;
            if (result.isSuccess) {
                callBack(result);
                that.updateCartCounterRequest();
            }
        });
    },

    removeFromCart: function (productId, callBack) {
        var that = this;
        $.post("/ShoppingCart/RemoveFromCart", {
            productId
        }, (result) => {
            if (result.isSuccess) {
                callBack(result);
                that.updateCartCounterRequest();
            }
        });
    },
    increaseCartItemQuantity: function (productId, callBack) {
        $.post("/ShoppingCart/IncreaseCartItemQuantity", {
            productId
        }, (result) => {
            if (result.isSuccess)
                callBack(result);
        });
    },
    decreaseCartItemQuantity: function (productId, callBack) {
        $.post("/ShoppingCart/DecreaseCartItemQuantity", {
            productId
        }, (result) => {
            if (result.isSuccess)
                callBack(result);
        });
    }
}

