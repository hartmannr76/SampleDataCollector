pipeline {
  agent {
    docker {
      image 'hartmannr76/dotnet-build-tools:2.0-sdk'
    }

  }
  stages {
    stage('Build') {
      parallel {
        stage('Build') {
          steps {
            sh 'make'
          }
        }
        stage('Run') {
          steps {
            echo 'Well hello'
          }
        }
      }
    }
    stage('Done') {
      steps {
        sh 'echo \'fin\''
      }
    }
  }
}