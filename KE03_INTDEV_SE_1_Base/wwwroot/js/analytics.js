// Analytics tracking with GDPR compliance
class WebsiteAnalytics {
    constructor() {
        this.consentGiven = false;
        this.analyticsData = [];
        this.init();
    }

    init() {
        // Check if analytics consent is given
        this.checkConsent();
        
        if (this.consentGiven) {
            this.setupTracking();
        }
    }

    checkConsent() {
        // In a real application, this would check the database
        // For now, we'll use localStorage as a simple example
        const consent = localStorage.getItem('analyticsConsent');
        this.consentGiven = consent === 'true';
    }

    setupTracking() {
        if (!this.consentGiven) return;

        // Track page views
        this.trackPageView();

        // Track user interactions
        this.trackProductViews();
        this.trackSearchBehavior();
        this.trackCartActions();
        this.trackFilterUsage();
    }

    trackPageView() {
        const pageData = {
            type: 'pageview',
            url: window.location.pathname,
            title: document.title,
            timestamp: new Date().toISOString(),
            userAgent: navigator.userAgent,
            screenSize: `${screen.width}x${screen.height}`,
            referrer: document.referrer
        };

        this.sendAnalyticsData(pageData);
    }

    trackProductViews() {
        // Track when users view product details
        document.addEventListener('click', (e) => {
            if (e.target.closest('.card-title')) {
                const productName = e.target.textContent;
                const productData = {
                    type: 'product_view',
                    product: productName,
                    timestamp: new Date().toISOString()
                };
                this.sendAnalyticsData(productData);
            }
        });
    }

    trackSearchBehavior() {
        // Track search terms
        const searchInput = document.getElementById('searchTerm');
        if (searchInput) {
            searchInput.addEventListener('input', (e) => {
                if (e.target.value.length > 2) {
                    const searchData = {
                        type: 'search',
                        term: e.target.value,
                        timestamp: new Date().toISOString()
                    };
                    this.sendAnalyticsData(searchData);
                }
            });
        }
    }

    trackCartActions() {
        // Track add to cart actions
        document.addEventListener('click', (e) => {
            if (e.target.classList.contains('btn-add-to-cart')) {
                const productCard = e.target.closest('.card');
                const productName = productCard.querySelector('.card-title').textContent;
                const quantity = productCard.querySelector('input[name="quantity"]').value;
                
                const cartData = {
                    type: 'add_to_cart',
                    product: productName,
                    quantity: parseInt(quantity),
                    timestamp: new Date().toISOString()
                };
                this.sendAnalyticsData(cartData);
            }
        });
    }

    trackFilterUsage() {
        // Track price filter usage
        document.addEventListener('change', (e) => {
            if (e.target.name === 'PriceRange') {
                const filterData = {
                    type: 'filter_usage',
                    filter: 'price',
                    value: e.target.value,
                    timestamp: new Date().toISOString()
                };
                this.sendAnalyticsData(filterData);
            }
        });
    }

    sendAnalyticsData(data) {
        if (!this.consentGiven) return;

        // In a real application, this would send to a server
        // For now, we'll store locally and log
        this.analyticsData.push(data);
        
        // Log to console for demonstration
        console.log('Analytics Event:', data);
        
        // Store in localStorage (in real app, send to server)
        localStorage.setItem('analyticsData', JSON.stringify(this.analyticsData));
    }

    // Method to get analytics data (for admin purposes)
    getAnalyticsData() {
        return this.analyticsData;
    }

    // Method to clear analytics data (for GDPR compliance)
    clearAnalyticsData() {
        this.analyticsData = [];
        localStorage.removeItem('analyticsData');
        console.log('Analytics data cleared for GDPR compliance');
    }

    // Method to update consent
    updateConsent(given) {
        this.consentGiven = given;
        localStorage.setItem('analyticsConsent', given.toString());
        
        if (given) {
            this.setupTracking();
        } else {
            this.clearAnalyticsData();
        }
    }
}

// Initialize analytics when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    window.websiteAnalytics = new WebsiteAnalytics();
});

// Export for use in other scripts
if (typeof module !== 'undefined' && module.exports) {
    module.exports = WebsiteAnalytics;
} 