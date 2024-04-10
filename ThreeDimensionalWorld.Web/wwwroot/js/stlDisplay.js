const scenes = []
const meshes = []
const lights = []

SetConatainerFiles();


function SetConatainerFiles() {
    const stlContainers = document.querySelectorAll(".stl-container")

    stlContainers.forEach(stlContainer => {

        let colorMaterial = stlContainer.getAttribute("data-3d-color");

        if (!colorMaterial) {
            colorMaterial = "#ffffff";
        }

        let backgroungColor = chooseBackgroundColor(colorMaterial);

        // Scene
        var scene = new THREE.Scene();
        scene.background = new THREE.Color(backgroungColor);

        scenes.push(scene)

        let parent = stlContainer.parentElement;

        let canvaWidth = stlContainer.offsetWidth

        while (canvaWidth === 0) {
            canvaWidth = parent.offsetWidth;
            parent = parent.parentElement;
        }

        parent = stlContainer.parentElement;

        let canvaHeight = stlContainer.offsetHeight
        while (canvaHeight === 0) {
            canvaHeight = parent.offsetHeight;
            parent = parent.parentElement;
        }


        // Camera
        var camera = new THREE.PerspectiveCamera(75, canvaWidth / canvaHeight, 0.1, 1000);

        // Renderer
        var renderer = new THREE.WebGLRenderer();

        renderer.setSize(canvaWidth, canvaHeight);
        stlContainer.appendChild(renderer.domElement);

        // Handle window resize
        window.addEventListener('resize', function () {

            parent = stlContainer.parentElement;

            canvaWidth = stlContainer.offsetWidth

            while (canvaWidth === 0) {
                canvaWidth = parent.offsetWidth;
                parent = parent.parentElement;
            }

            parent = stlContainer.parentElement;

            canvaHeight = stlContainer.offsetHeight
            while (canvaHeight === 0) {
                canvaHeight = parent.offsetHeight;
                parent = parent.parentElement;
            }

            var width = canvaWidth;
            var height = canvaHeight;
            renderer.setSize(width, height);
            camera.aspect = width / height;
            camera.updateProjectionMatrix();
        });

        // Ambient light
        var ambientLight = new THREE.AmbientLight(0xffffff, 0.5);
        scene.add(ambientLight);

        // Directional light
        var directionalLight = new THREE.DirectionalLight(0xffffff, 0.5);
        scene.add(directionalLight);


        // Add OrbitControls
        var controls = new THREE.OrbitControls(camera, renderer.domElement);

        controls.enablePan = false;

        // STL Loader
        var loader = new THREE.STLLoader();

        controls.autoRotate = true
        controls.autoRotateSpeed = 10;
        


        // Load STL file
        loader.load(stlContainer.getAttribute("data-3d-src"), function (geometry) {
            var material = new THREE.MeshPhongMaterial({ color: colorMaterial, reflectivity: 0, shininess: 0 });
            var mesh = new THREE.Mesh(geometry, material);
            mesh.rotation.x = -Math.PI / 2;
            mesh.rotation.z = Math.PI;

            scene.add(mesh);

            meshes.push(mesh)

            // Ensure the model is centered
            geometry.center();

            // Update the camera to fit the object
            var boundingBox = new THREE.Box3().setFromObject(mesh);
            var center = new THREE.Vector3();
            boundingBox.getCenter(center);
            var size = new THREE.Vector3();
            boundingBox.getSize(size);

            // Update the camera position to be in front of the object
            var objectPosition = mesh.position.clone();
            var objectSize = new THREE.Vector3();
            boundingBox.getSize(objectSize);
            var cameraDistance = Math.max(objectSize.x, objectSize.y, objectSize.z) * 1.5; // You can adjust this multiplier as needed

            var cameraDirection = camera.position.clone().sub(objectPosition).normalize();
            camera.position.copy(objectPosition).add(cameraDirection.multiplyScalar(cameraDistance));

            controls.target.copy(center);

            camera.position.copy(center.clone().add(new THREE.Vector3(0, 0, cameraDistance))); // Adjust the camera distance as needed

            // Look at the center of the object
            camera.lookAt(center);



            // Update the rendering
            animate();
        });

        // Animation loop
        function animate() {
            directionalLight.position.copy(camera.position);
            controls.update();
            requestAnimationFrame(animate);
            renderer.render(scene, camera);
        }



    })
}

function hexToNumeric(hex) {
    if (!hex) {
        return
    }

    // Remove the '#' character if it exists
    hex = hex.replace('#', '');

    // Parse the hexadecimal string to numeric value
    var numericValue = parseInt(hex, 16);

    return numericValue;
}

function UpdateColor(color) {

    if (!color) {
        return
    }

    meshes.forEach(m => {
        m.material.color.set(hexToNumeric(color));
        m.material.shadowSide = THREE.DoubleSide; // Adjust this according to your needs
        m.material.receiveShadow = true;
        m.material.castShadow = true;
    })

    lights.forEach(l => {
        l.color.set(color)
    })

    scenes.forEach(scene => {
        scene.background = new THREE.Color(chooseBackgroundColor(color));
    })
}

function chooseBackgroundColor(colorString) {
    // Remove '#' if present
    colorString = colorString.replace('#', '');

    // Convert hexadecimal color to decimal
    var decimalColor = parseInt(colorString, 16);

    // Extract the individual color components (R, G, B)
    var red = (decimalColor >> 16) & 255;
    var green = (decimalColor >> 8) & 255;
    var blue = decimalColor & 255;

    // Calculate luminance using the relative luminance formula
    var luminance = (0.2126 * red + 0.7152 * green + 0.0722 * blue) / 255;

    // Threshold for deciding when white is not ok
    var threshold = 0.5; // Adjust this threshold as needed

    // Check if luminance is above the threshold
    if (luminance > threshold) {
        // If too bright, return black
        return '#000000';
    } else {
        // If not too bright, return white
        return '#ffffff';
    }
}