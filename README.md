# CryptoMM
This application is based on WPF .NET, is designed to encrypt/decrypt data using AES 256 bits by means of key and IV transmission via RSA 1024 bits.The generation of QR codes, keys, and PEM certificates with the ability to save is also supported.
### How is work
1) In the "Certificates and keys" tab for encryption/decryption to work, you at least need to get the secret key and IV (Initialization vector) in the form of a Base64 string. This key and IV can be decrypted from the RSA cipher using the "Decrypt AES" button. To do this, import the RSA public and secret certificate.
2) In the "Encryption" tab, you can encrypt data (By default it is Latin, but other languages are supported by specifying the appropriate check mark) and get a cipher in the form of a Base64 string.
3) In the "Decryption" tab, you can decrypt the data from the Base64 string, specifying the desired language.
4) In the Generation tab, you can generate an RSA public/secret key pair in the PEM certificate format, as well as an AES secret key and an IV in the PEM certificate format.
5) In the "QR" tab, you can generate a QR code from a file by pre-importing it. The QR code will appear immediately. You can also manually enter the data and generate a QR code (UTF-8 support)
6) The "Info" tab contains information about the application.
<p>
  <img src="https://raw.githubusercontent.com/marwin1991/profile-technology-icons/refs/heads/main/icons/_net_core.png" height="100px" width="100px">
  <img src="https://raw.githubusercontent.com/marwin1991/profile-technology-icons/refs/heads/main/icons/c%23.png" height="100px" width="100px">
  <img src="https://raw.githubusercontent.com/marwin1991/profile-technology-icons/refs/heads/main/icons/windows.png" height="100px" width="100px">
</p>
