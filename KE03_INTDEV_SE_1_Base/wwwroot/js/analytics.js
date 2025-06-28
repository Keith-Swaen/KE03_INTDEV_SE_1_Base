// Analytics tracking met AVG compliance voor het bijhouden van gebruikersgedrag
class WebsiteAnalytics {
    // Constructor die de analytics class initialiseert
    constructor() {
        // Geeft aan of de gebruiker toestemming heeft gegeven voor analytics
        this.consentGiven = false;
        // Array om alle analytics data op te slaan
        this.analyticsData = [];
        this.init();
    }

    // Initialiseert de analytics tracking
    init() {
        // Controleert of analytics toestemming is gegeven
        this.checkConsent();
        
        // Als toestemming is gegeven, start de tracking
        if (this.consentGiven) {
            this.setupTracking();
        }
    }

    // Controleert of de gebruiker toestemming heeft gegeven voor analytics
    checkConsent() {
        // In een echte applicatie zou dit de database controleren
        // Voor nu gebruik ik localStorage als voorbeeld
        const consent = localStorage.getItem('analyticsConsent');
        this.consentGiven = consent === 'true';
    }

    // Stelt alle tracking functionaliteit in als toestemming is gegeven
    setupTracking() {
        if (!this.consentGiven) return;

        // Volgt paginaweergaven
        this.trackPageView();

        // Volgt gebruikersinteracties
        this.trackProductViews();
        this.trackSearchBehavior();
        this.trackCartActions();
        this.trackFilterUsage();
    }

    // Volgt wanneer een gebruiker een pagina bekijkt
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

    // Volgt wanneer gebruikers productdetails bekijken
    trackProductViews() {
        // Volgt wanneer gebruikers op producttitels klikken
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

    // Volgt zoekgedrag van gebruikers
    trackSearchBehavior() {
        // Volgt zoektermen die gebruikers invoeren
        const searchInput = document.getElementById('searchTerm');
        if (searchInput) {
            searchInput.addEventListener('input', (e) => {
                // Alleen tracken als er meer dan 2 karakters zijn ingevoerd
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

    // Volgt acties in de winkelwagen
    trackCartActions() {
        // Volgt wanneer producten aan de winkelwagen worden toegevoegd
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

    // Volgt gebruik van filters
    trackFilterUsage() {
        // Volgt wanneer gebruikers prijsfilters gebruiken
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

    // Verzendt analytics data, alleen als toestemming is gegeven
    sendAnalyticsData(data) {
        if (!this.consentGiven) return;

        // In een echte applicatie zou dit naar een server worden verzonden
        // Voor nu sla ik het lokaal op en log ik het
        this.analyticsData.push(data);
        
        // Log naar console voor demonstratie
        console.log('Analytics Event:', data);
        
        // Sla op in localStorage
        localStorage.setItem('analyticsData', JSON.stringify(this.analyticsData));
    }

    // Methode om analytics data op te halen zodat admins kunnen zien wat gebruikers doen
    getAnalyticsData() {
        return this.analyticsData;
    }

    // Methode om analytics data te wissen wanneer gebruiker dat vraagt volgens AVG wet
    clearAnalyticsData() {
        this.analyticsData = [];
        localStorage.removeItem('analyticsData');
        console.log('Analytics data gewist voor AVG compliance');
    }

    // Methode om toestemming bij te werken
    updateConsent(given) {
        this.consentGiven = given;
        localStorage.setItem('analyticsConsent', given.toString());
        
        // Als toestemming wordt gegeven, start tracking
        if (given) {
            this.setupTracking();
        } else {
            // Als toestemming wordt ingetrokken, wis alle data
            this.clearAnalyticsData();
        }
    }
}

// Start analytics tracking nadat alle HTML elementen zijn geladen op de pagina
document.addEventListener('DOMContentLoaded', () => {
    window.websiteAnalytics = new WebsiteAnalytics();
});

// Export voor gebruik in andere scripts
if (typeof module !== 'undefined' && module.exports) {
    module.exports = WebsiteAnalytics;
} 