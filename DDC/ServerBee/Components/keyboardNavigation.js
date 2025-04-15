window.keyboardNavigation = {
    initialize: function (dotnetReference) {
        document.addEventListener('keydown', function (e) {
            // Prevent default for specific keys to avoid browser navigation
            if (['ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight', 'Space'].includes(e.key)) {
                e.preventDefault();
                
                // Call .NET method with the key info
                dotnetReference.invokeMethodAsync('HandleKeyboardEvent', e.key, e.ctrlKey, e.shiftKey);
            }
        });
    },
    
    setFocus: function (elementId) {
        const element = document.getElementById(elementId);
        if (element) {
            element.focus();
        }
    }
};