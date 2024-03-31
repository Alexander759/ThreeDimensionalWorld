class Slideshow {
    constructor(slideshowDOM) {
        this.slideshowDOM = slideshowDOM;

        if (slideshowDOM.style.display !== "none") {
            this.start();
        }
    }

    start() {
        this.slideIndex = 1;
        this.slides = Array.from(this.slideshowDOM.getElementsByClassName("mySlides"));
        this.dots = Array.from(this.slideshowDOM.getElementsByClassName("dot"));



        this.prev = this.slideshowDOM.querySelector(".prev");
        this.next = this.slideshowDOM.querySelector(".next");

        this.dots.forEach((item, i) => {
            item.addEventListener("click", () => this.currentSlide(i + 1));
        });

        if (this.prev.getAttribute('data-slideshow-listener') !== "true") {

            this.prev.addEventListener("click", () => this.plusSlides(-1));
            this.prev.setAttribute("data-slideshow-listener", "true")
        }

        if (this.next.getAttribute('data-slideshow-listener') !== "true") {
            console.log(this.next)
            this.next.addEventListener("click", () => this.plusSlides(1));
            this.next.setAttribute("data-slideshow-listener", "true")
        }

        this.showSlides(this.slideIndex);
    }

    plusSlides(n) {
        this.showSlides(this.slideIndex += n);
    }

    currentSlide(n) {
        this.showSlides(this.slideIndex = n);
    }

    showSlides(n) {
        let i;
        if (n > this.slides.length) { this.slideIndex = 1; }
        if (n < 1) { this.slideIndex = this.slides.length; }
        for (i = 0; i < this.slides.length; i++) {
            this.slides[i].style.display = "none";
        }

        for (i = 0; i < this.dots.length; i++) {
            this.dots[i].className = this.dots[i].className.replace(" active", "");
        }
        this.slides[this.slideIndex - 1].style.display = "block";
        this.dots[this.slideIndex - 1].className += " active";
    }
}


const slideshows = Array.from(document.querySelectorAll(".slideshow")).map(item => {
    return new Slideshow(item)
})