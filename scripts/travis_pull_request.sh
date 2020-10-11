echo "Travis pull_request job"

# Download dependencies and build
dotnet publish functions

# Preview changes that would be made if the PR were merged.
case ${TRAVIS_BRANCH} in
    master)
        pulumi stack select dev
        pulumi preview
        ;;
    production)
        pulumi stack select dev
        pulumi preview
        ;;
    *)
        echo "No Pulumi stack targeted by pull request branch ${TRAVIS_BRANCH}."
        ;;
esac