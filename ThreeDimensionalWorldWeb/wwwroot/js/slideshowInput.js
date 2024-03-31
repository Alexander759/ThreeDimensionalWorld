const slideshowDotsContainer = document.querySelector(".slideshow-dots-container")
const slideshowContainer = document.querySelector(".slideshow-container")

const fileInput = document.querySelector("#files-input")
fileInput.addEventListener('change', function (event) {

    document.querySelector(".slideshow").style.display = "block";

    const fileList = event.target.files;
    console.log('Files selected:', fileList);

    slideshowContainer.replaceChildren(slideshowContainer.querySelector(".prev"), slideshowContainer.querySelector(".next"))

    slideshowDotsContainer.replaceChildren();

    Array.from(fileList).map((item, i) => {
        const span = document.createElement('span');
        span.classList.add("dot");
        return span
    }).forEach(item => slideshowDotsContainer.appendChild(item))

    for (const file of fileList) {
        const fileExtension = file.name.substr(file.name.lastIndexOf('.')).toLowerCase();
        console.log(file)
        if (fileExtension == ".stl") {

            const div = document.createElement('div');
            div.classList.add("mySlides");
            div.classList.add("fade-slideshow");

            const div3d = document.createElement('div');
            div3d.classList.add("stl-container")
            div3d.setAttribute("data-3d-src", URL.createObjectURL(file));

            div.appendChild(div3d);

            slideshowContainer.appendChild(div);

        }
        else {
            // Create image element
            const img = document.createElement('img');
            img.src = URL.createObjectURL(file);

            const div = document.createElement('div');
            div.classList.add("mySlides")
            div.classList.add("fade-slideshow")

            div.appendChild(img)

            slideshowContainer.appendChild(div)
        }
    }

    SetConatainerFiles();
    slideshows[0].start()
});