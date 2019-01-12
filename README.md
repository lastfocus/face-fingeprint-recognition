# face-fingeprint-recognition
Face and fingerprint recognition project that encode user data and decode its after succesfull face/fingeprint authentication process

   Coulde be usefull for students and developers looking for simple c# face/fingerprint recognition app
Project was written in 2014 as prototype/example not for production reasons and now in archive mode. Some code parts could be outdated.
PR are welcomed.

using:
* sqlce db to store users data (face frames, fingerprint frames, password hashes and other)
* aes to encrypt user data
* different methods for face recognition based on EMGU CV (.Net wrapper for opencv library)
* SourceAFIS framework for fingerprint matcher
