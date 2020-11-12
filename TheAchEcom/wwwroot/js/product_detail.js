
const PRODUCT_DETAIL_CONTROLS = {
    cart: {
        updateControl: function (item, isAddedToCart) {
            $(item).attr('added-to-cart', isAddedToCart);
            var text = (isAddedToCart) ? "Remove From Cart" : "Add To Cart";
            debugger
            $(item).html(text);
        },
        onClick: function (item, productId) {
            var isAddedToCart = $(item).attr('added-to-cart') == 'true' ? true : false;
            if (isAddedToCart == true) {
                CART_MANAGER.removeFromCart(productId,
                    () => this.updateControl(item, !isAddedToCart))
            }
            else {
                CART_MANAGER.addToCart(productId,
                    () => this.updateControl(item, !isAddedToCart));
            }
        }
    }
};

function OnProductDetailControlClick(name, item, productId) {
    PRODUCT_DETAIL_CONTROLS[name].onClick(item, productId);
}