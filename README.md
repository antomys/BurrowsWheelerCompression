<!-- Improved compatibility of back to top link: See: https://github.com/othneildrew/Best-README-Template/pull/73 -->
<a id="readme-top"></a>

<!-- PROJECT SHIELDS -->
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]
[![LinkedIn][linkedin-shield]][linkedin-url]


<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/antomys/Compression-Algorithms">
    <img src="assets/icon.png" alt="Logo" width="200" height="200">
  </a>

<h3 align="center">Compression Algorithms</h3>

  <p align="center">
    This repository houses a collection of implemented compression algorithms along with demo projects that showcase their functionality.
    It serves as a resource for developers interested in data compression techniques and their practical applications.
    <br />
    <a href="https://github.com/antomys/Compression-Algorithms"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/antomys/Compression-Algorithms">View Demo</a>
    ·
    <a href="https://github.com/antomys/Compression-Algorithms/issues/new?labels=bug&template=bug-report---.md">Report Bug</a>
    ·
    <a href="https://github.com/antomys/Compression-Algorithms/issues/new?labels=enhancement&template=feature-request---.md">Request Feature</a>
  </p>
</div>


<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <a href="#roadmap">Roadmap</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

This project is part of my bachelor diploma work. See [Diploma papers](compression-papers.pdf).
This project is a library of compression algorithms implemented in code. It serves two main purposes:

- To provide a collection of ready-to-use compression algorithms for developers and researchers.
- To demonstrate the practical application of these algorithms through demo projects.

The repository includes:

- A variety of compression algorithms, each implemented and optimized for practical use
- Demo projects that showcase how these algorithms work in real-world scenarios
- Documentation to help users understand and utilize the implemented algorithms

Whether you're a developer looking to integrate compression into your project, a student learning about data compression, or a researcher exploring different compression techniques, this repository aims to be a valuable resource.
<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Roadmap
- [X] Implement standard Lempel–Ziv–Welch (LZW) algorithm;
- [x] Implement standard Huffman algorithm;
- [x] Implement standard Burrows-Wheeler transformation:
    - [x] Implement custom query;
    - [x] Implement Suffix Array;
- [x] Implement Move-To-Front (MTF) algorithm;
- [x] Implement Run-Length-Encoding (RLE) algorithm;
- [x] Modify Lzw with Bwt without anything else;
- [x] Modify Huffman with Bwt without anything else;
- [x] Develop and implement custom Bwt + Mtf + Lzw algorithm;
- [x] Develop and implement custom Bwt + Mtf + Huffman algorithm;
- [x] Add demo projects;
- [x] Add statistics;

### Built With

* [![NET][NET]][NET-url]

<p align="right">(<a href="#readme-top">back to top</a>)</p>


<!-- GETTING STARTED -->
## Getting Started

Welcome to the Curious Benchmarks! This repository is designed to help you explore and understand performance characteristics of various .NET approaches. Follow these steps to get started:
### Prerequisites

- Install [.NET SDK](https://dotnet.microsoft.com/en-us/download) (version 8.0 or later)
- Install [BenchmarkDotNet](https://benchmarkdotnet.org/articles/overview.html) (**Included** in the project)


### Installation

1. **Clone the Repository**
    ```shell
   git clone https://github.com/antomys/Compression-Algorithms.git
   cd .\src\main
   ```

2. **Build the Solution**
    ```shell
   dotnet build
   ```

3. **Run Benchmarks.** Navigate to a specific benchmark project and run:
    ```shell
   dotnet run -c Release
   ```
<p align="right">(<a href="#readme-top">back to top</a>)</p>

If you have any questions or suggestions, please open an issue in the repository.


<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- CONTACT -->
## Contact

Ihor Volokhovych - [@incomingwebhook](https://t.me/incomingwebhook) - igorvolokhovych@gmail.com

Project Link: [https://github.com/antomys/Compression-Algorithms](https://github.com/antomys/Compression-Algorithms)

<p align="right">(<a href="#readme-top">back to top</a>)</p>


<!-- ACKNOWLEDGMENTS -->
## Acknowledgments

<p align="right">(<a href="#readme-top">back to top</a>)</p>


<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/antomys/Compression-Algorithms.svg?style=for-the-badge
[contributors-url]: https://github.com/antomys/Compression-Algorithms/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/antomys/Compression-Algorithms.svg?style=for-the-badge
[forks-url]: https://github.com/antomys/Compression-Algorithms/network/members
[stars-shield]: https://img.shields.io/github/stars/antomys/Compression-Algorithms.svg?style=for-the-badge
[stars-url]: https://github.com/antomys/Compression-Algorithms/stargazers
[issues-shield]: https://img.shields.io/github/issues/antomys/Compression-Algorithms.svg?style=for-the-badge
[issues-url]: https://github.com/antomys/Compression-Algorithms/issues
[license-shield]: https://img.shields.io/github/license/antomys/Compression-Algorithms.svg?style=for-the-badge
[license-url]: https://github.com/antomys/Compression-Algorithms/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://www.linkedin.com/in/ihor-volokhovych-23875217a/
[NET]: https://img.shields.io/badge/-.NET%208.0-blueviolet
[NET-url]: https://dotnet.microsoft.com/en-us/
