const scenes = []
const meshes = []
const lights = []

SetConatainerFiles();


function SetConatainerFiles() {
    const stlContainers = document.querySelectorAll(".stl-container")

    stlContainers.forEach(stlContainer => {

        let color = stlContainer.getAttribute("data-3d-color");
        let colorMaterial = null;

        if (!color) {
            color = "#ffffff";
            colorMaterial = "#ffffff";
        } else {
            colorMaterial = color;
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

        renderer.outputEncoding = THREE.LinearSRGBColorSpace

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
        //controls.addEventListener('change', render);
        /*controls.addEventListener('change', () => {
            directionalLight.position.set(camera.position.x, camera.position.y, camera.position.z);
        });*/

        // STL Loader
        var loader = new THREE.STLLoader();

        controls.autoRotate = true
        controls.autoRotateSpeed = 10;
        


        // Load your STL file
        loader.load(stlContainer.getAttribute("data-3d-src"), function (geometry) {
            var material = new THREE.MeshPhongMaterial({ color: colorMaterial, reflectivity: 0, shininess: 0 });
            var mesh = new THREE.Mesh(geometry, material);
            mesh.rotation.x = -Math.PI / 2;
            mesh.rotation.z = Math.PI;
            console.log(mesh)

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
            var maxDim = Math.max(size.x, size.y, size.z);
            var fov = camera.fov * (Math.PI / 180);
            /*var cameraZ = Math.abs(maxDim / 4 * Math.tan(fov * 2));



            camera.position.z = cameraZ;*/

            // Update the camera position to be in front of the object
            var objectPosition = mesh.position.clone();
            var objectSize = new THREE.Vector3();
            boundingBox.getSize(objectSize);
            var cameraDistance = Math.max(objectSize.x, objectSize.y, objectSize.z) * 1.5; // You can adjust this multiplier as needed

            var cameraDirection = camera.position.clone().sub(objectPosition).normalize();
            camera.position.copy(objectPosition).add(cameraDirection.multiplyScalar(cameraDistance));

            console.log(center)
            console.log(size)
            console.log(objectSize)

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



// Simple three.js example

/*import * as THREE from "https://threejs.org/build/three.module.js";
import { OrbitControls } from "https://threejs.org/examples/jsm/controls/OrbitControls.js";*/

/*let scene, camera, renderer, controls, threeContainer;

function init() {
    threeContainer = document.querySelector(".stl-container");

    scene = new THREE.Scene();
    scene.background = new THREE.Color(0xdddddd);

    createCamera()
    createLights();
    createMeshes()
    createRenderer();
    createControls();

    renderer.setAnimationLoop(() => {

        update();
        render();

    });

}

function createCamera() {
    camera = new THREE.PerspectiveCamera(40, window.innerWidth / window.innerHeight, 1, 5000);

    camera.rotation.y = 45 / 180 * Math.PI;
    camera.position.x = 10;
    camera.position.y = 10;
    camera.position.z = 10;
}

function createLights() {
    const ambientLight = new THREE.HemisphereLight(
        0xddeeff, // sky color
        0x202020, // ground color
        5, // intensity
    );

    const mainLight = new THREE.DirectionalLight(0xffffff, 5);
    mainLight.position.set(10, 10, 10);

    scene.add(ambientLight, mainLight);
}

function createMeshes() {
    const geometry = new THREE.BoxGeometry(1, 1, 1);
    const mat = new THREE.MeshNormalMaterial();
    const cube = new THREE.Mesh(geometry, mat);
    scene.add(cube);

    cube.position.x = 5;
    cube.position.z = -5;

}

function createRenderer() {

    renderer = new THREE.WebGLRenderer({ antialias: true });
    renderer.setClearColor(0xdddddd)
    renderer.setSize(window.innerWidth, window.innerHeight);

    document.body.appendChild(renderer.domElement);

    renderer.gammaFactor = 2.2;
    renderer.gammaOutput = true;

    renderer.physicallyCorrectLights = true;

    threeContainer.appendChild(renderer.domElement);
}

function createControls() {
    controls = new OrbitControls(camera, threeContainer);
}

function update() {
    // update elements in this function
}

// render, or 'draw a still image', of the scene
function render() {
    renderer.render(scene, camera);
}

window.addEventListener('resize', () => {
    renderer.setSize(window.innerWidth, window.innerHeight);
    camera.aspect = window.innerWidth / window.innerHeight

    camera.updateProjectionMatrix();
})

init();*/