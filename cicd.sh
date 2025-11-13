#!/bin/bash
#
# DotNet CI/CD Automation Script
#
# Usage: ./cicd.sh {command}
# Commands: all, build, test, run, clean, setup, report, help
#

# --- Configuration Variables ---
API_PROJECT_DIR="./api"                 
TEST_PROJECT_DIR="./Api.Tests"          
SLN_FILE="./api.sln"                    
REPORT_BASE_DIR="./TestResults"
TRX_REPORT_FILE="test_results.trx"
COVERAGE_REPORT_FILE="coverage.cobertura.xml"
HTML_REPORT_SUBDIR="html"
HTML_REPORT_FILE="index.html"

# Define colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[0;33m'
NC='\033[0m' # No Color

# --- Environment Detection ---
detect_environment() {
    case "$(uname -s)" in
        Linux*)     echo "linux";;
        Darwin*)    echo "macos";;
        CYGWIN*|MINGW*|MSYS*) echo "windows";;
        *)          echo "unknown"
    esac
}

OS_ENV=$(detect_environment)

# --- Helper Functions ---

status_message() {
    local color="$1"
    local message="$2"
    echo -e "\n${color}=====================================================${NC}"
    echo -e "${color}  ${message}${NC}"
    echo -e "${color}=====================================================${NC}\n"
}

convert_to_win_path() {
    local shell_path="$1"
    local converted_path="${shell_path}"

    if command -v cygpath >/dev/null 2>&1; then
        converted_path=$(cygpath -w "${shell_path}")
    elif command -v wslpath >/dev/null 2>&1; then
        converted_path=$(wslpath -w "${shell_path}")
    fi
    echo "${converted_path}"
}

validate_configuration() {
    status_message "$YELLOW" "Validating project structure..."
    
    if [ ! -d "${API_PROJECT_DIR}" ]; then
        status_message "$RED" "Error: API project directory not found: ${API_PROJECT_DIR}"
        return 1
    fi
    
    if [ ! -d "${TEST_PROJECT_DIR}" ]; then
        status_message "$RED" "Error: Test project directory not found: ${TEST_PROJECT_DIR}"
        return 1
    fi
    
    if ! ls "${API_PROJECT_DIR}"/*.csproj >/dev/null 2>&1 && ! ls "${API_PROJECT_DIR}"/*.fsproj >/dev/null 2>&1; then
        status_message "$RED" "Error: No project file found in API directory: ${API_PROJECT_DIR}"
        return 1
    fi
    
    if ! ls "${TEST_PROJECT_DIR}"/*.csproj >/dev/null 2>&1 && ! ls "${TEST_PROJECT_DIR}"/*.fsproj >/dev/null 2>&1; then
        status_message "$RED" "Error: No project file found in Test directory: ${TEST_PROJECT_DIR}"
        return 1
    fi
    
    status_message "$GREEN" "Project structure validation passed."
    return 0
}

check_dependencies() {
    status_message "$YELLOW" "Checking dependencies..."
    
    if ! command -v dotnet &> /dev/null; then
        status_message "$RED" "Error: .NET SDK is not installed or not in PATH"
        return 1
    fi
    
    local dotnet_version=$(dotnet --version 2>/dev/null)
    status_message "$GREEN" "Using .NET SDK: ${dotnet_version}"
    return 0
}

# --- Tool Installation ---

install_report_generator() {
    if ! command -v reportgenerator &> /dev/null; then
        status_message "$YELLOW" "ReportGenerator not found. Installing dotnet global tool (ReportGenerator)..."
        dotnet tool install -g ReportGenerator
        if [ $? -ne 0 ]; then
            status_message "$RED" "Failed to install ReportGenerator. HTML report generation will fail."
            return 1
        fi
        status_message "$GREEN" "ReportGenerator installed successfully."
    else
        status_message "$GREEN" "ReportGenerator is already installed."
    fi
    return 0
}

# Clean up ALL test results completely
clean_test_results() {
    status_message "$YELLOW" "Cleaning up test results directory..."
    if [ -d "${REPORT_BASE_DIR}" ]; then
        rm -rf "${REPORT_BASE_DIR}"
        status_message "$GREEN" "Test results directory removed: ${REPORT_BASE_DIR}"
    fi
    mkdir -p "${REPORT_BASE_DIR}"
}

# Function to find and consolidate coverage files
find_and_consolidate_coverage() {
    local target_file="$1"
    local search_dir="$2"
    
    status_message "$YELLOW" "Searching for coverage files in: ${search_dir}"
    
    # Find all cobertura.xml files, excluding the target directory itself
    local coverage_files=$(find "${search_dir}" -name "*.cobertura.xml" -type f ! -path "*/TestReport/*" | head -5)
    
    if [ -z "${coverage_files}" ]; then
        status_message "$RED" "No coverage files found in ${search_dir}"
        return 1
    fi
    
    echo "Found coverage files:"
    echo "${coverage_files}"
    
    # Use the first found coverage file (they should be identical)
    local first_coverage=$(echo "${coverage_files}" | head -1)
    
    if [ -n "${first_coverage}" ] && [ -f "${first_coverage}" ]; then
        status_message "$GREEN" "Using coverage file: ${first_coverage}"
        
        # Copy to target location
        mkdir -p "$(dirname "${target_file}")"
        cp "${first_coverage}" "${target_file}"
        status_message "$GREEN" "Coverage file consolidated to: ${target_file}"
        return 0
    else
        status_message "$RED" "Could not find a valid coverage file"
        return 1
    fi
}

generate_html_report() {
    local coverage_file="$1"
    local output_dir="$2"
    
    status_message "$YELLOW" "Generating HTML coverage report..."
    
    if [ ! -f "${coverage_file}" ]; then
        status_message "$RED" "Error: Coverage file not found: ${coverage_file}"
        return 1
    fi
    
    local coverage_win_path=$(convert_to_win_path "${coverage_file}")
    local output_win_path=$(convert_to_win_path "${output_dir}")
    
    reportgenerator \
        -reports:"${coverage_win_path}" \
        -targetdir:"${output_win_path}" \
        -reporttypes:Html
    
    if [ $? -eq 0 ]; then
        status_message "$GREEN" "HTML coverage report generated successfully."
        return 0
    else
        status_message "$RED" "Failed to generate HTML coverage report."
        return 1
    fi
}

# --- Core Functions ---

restore_dependencies() {
    status_message "$YELLOW" "Restoring dependencies..."
    dotnet restore ${SLN_FILE}
    if [ $? -eq 0 ]; then
        status_message "$GREEN" "Restore successful."
        return 0
    else
        status_message "$RED" "Dependency restore failed. Exiting."
        return 1
    fi
}

clean_projects() {
    status_message "$YELLOW" "Cleaning projects..."
    dotnet clean ${SLN_FILE}
    if [ $? -eq 0 ]; then
        status_message "$GREEN" "Clean successful."
        return 0
    else
        status_message "$RED" "Clean failed."
        return 1
    fi
}

build_solution() {
    status_message "$YELLOW" "Building solution in Release configuration..."
    dotnet build ${SLN_FILE} -c Release --no-restore
    if [ $? -eq 0 ]; then
        status_message "$GREEN" "Build successful. Artifacts ready."
        return 0
    else
        status_message "$RED" "Build failed. Check the errors above."
        return 1
    fi
}

# Clean up extra directories after tests
clean_extra_directories() {
    status_message "$YELLOW" "Cleaning up extra directories..."
    
    # Remove all directories except our report subdirectory
    find "${REPORT_BASE_DIR}" -mindepth 1 -maxdepth 1 -type d ! -name "${HTML_REPORT_SUBDIR}" -exec rm -rf {} + 2>/dev/null || true
    
    status_message "$GREEN" "Extra directories cleaned up."
}

# Show final file structure (CLEAN VERSION - no file spam)
show_final_structure() {
    status_message "$GREEN" "Test Results Summary:"
    echo "Location: ${REPORT_BASE_DIR}"
    echo ""
    
    # Show only the main directory structure without listing all files
    if command -v tree >/dev/null 2>&1; then
        # Use tree with limited depth and file limit
        tree "${REPORT_BASE_DIR}" -L 2 2>/dev/null | head -20
    else
        # Fallback: show directory structure without file spam
        echo "📁 TestResults/"
        if [ -f "${REPORT_BASE_DIR}/${TRX_REPORT_FILE}" ]; then
            echo "   📄 ${TRX_REPORT_FILE}"
        fi
        if [ -f "${REPORT_BASE_DIR}/${COVERAGE_REPORT_FILE}" ]; then
            echo "   📄 ${COVERAGE_REPORT_FILE}"
        fi
        if [ -d "${REPORT_BASE_DIR}/${HTML_REPORT_SUBDIR}" ]; then
            echo "   📁 ${HTML_REPORT_SUBDIR}/ (HTML coverage report)"
            if [ -f "${REPORT_BASE_DIR}/${HTML_REPORT_SUBDIR}/${HTML_REPORT_FILE}" ]; then
                echo "      📄 ${HTML_REPORT_FILE} (main report)"
            fi
        fi
    fi
    
    echo ""
    echo "Essential files created:"
    if [ -f "${REPORT_BASE_DIR}/${TRX_REPORT_FILE}" ]; then
        echo "  ✅ ${TRX_REPORT_FILE} - Test results"
    fi
    if [ -f "${REPORT_BASE_DIR}/${COVERAGE_REPORT_FILE}" ]; then
        echo "  ✅ ${COVERAGE_REPORT_FILE} - Coverage data"
    fi
    if [ -f "${REPORT_BASE_DIR}/${HTML_REPORT_SUBDIR}/${HTML_REPORT_FILE}" ]; then
        echo "  ✅ HTML Report - ${REPORT_BASE_DIR}/${HTML_REPORT_SUBDIR}/${HTML_REPORT_FILE}"
    fi
}

# Method 1: Using Coverlet directly for cleaner output
run_tests_coverlet() {
    status_message "$YELLOW" "Running tests with Coverlet (clean method)..."
    
    # Clean up completely
    clean_test_results
    
    # Define direct paths
    local trx_path="${REPORT_BASE_DIR}/${TRX_REPORT_FILE}"
    local coverage_path="${REPORT_BASE_DIR}/${COVERAGE_REPORT_FILE}"
    local html_report_dir="${REPORT_BASE_DIR}/${HTML_REPORT_SUBDIR}"
    
    echo "TRX Report: ${trx_path}"
    echo "Coverage Report: ${coverage_path}"
    echo "HTML Report: ${html_report_dir}/${HTML_REPORT_FILE}"

    # Run tests with Coverlet for direct control
    dotnet test ${TEST_PROJECT_DIR} \
        --configuration Release \
        --no-build \
        --verbosity normal \
        --logger "trx;LogFileName=${trx_path}" \
        --collect:"XPlat Code Coverage" \
        --results-directory "${REPORT_BASE_DIR}" \
        --settings .runsettings

    local test_exit_code=$?

    if [ ${test_exit_code} -eq 0 ]; then
        status_message "$GREEN" "Tests successful. Processing reports..."
        
        # Consolidate coverage file
        if find_and_consolidate_coverage "${coverage_path}" "${REPORT_BASE_DIR}"; then
            # Generate HTML report
            if install_report_generator && generate_html_report "${coverage_path}" "${html_report_dir}"; then
                # Try to open the report
                local html_report_path="${html_report_dir}/${HTML_REPORT_FILE}"
                if [ -f "${html_report_path}" ]; then
                    open_report_in_browser "${html_report_path}"
                fi
            fi
        fi
        
        # Clean up extra directories (keep only what we need)
        clean_extra_directories
        
        # Show final file structure (clean version)
        show_final_structure
        
    else
        status_message "$RED" "Tests failed. Review the results."
        return 1
    fi

    return ${test_exit_code}
}

# Method 2: Using runsettings with explicit control
run_tests_runsettings() {
    status_message "$YELLOW" "Running tests with runsettings (explicit control)..."
    
    # Clean up completely
    clean_test_results
    
    # Create runsettings file with absolute control
    local runsettings_file="${REPORT_BASE_DIR}/test.runsettings"
    cat > "${runsettings_file}" << EOF
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <RunConfiguration>
    <ResultsDirectory>${REPORT_BASE_DIR}</ResultsDirectory>
  </RunConfiguration>
  <DataCollectionRunSettings>
    <DataCollectors>
      <DataCollector friendlyName="XPlat code coverage">
        <Configuration>
          <Format>cobertura</Format>
          <OutputPath>${REPORT_BASE_DIR}</OutputPath>
          <FileName>${COVERAGE_REPORT_FILE}</FileName>
          <Exclude>[*]*.Migrations.*</Exclude>
          <ExcludeByFile>**/Migrations/**</ExcludeByFile>
        </Configuration>
      </DataCollector>
    </DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
EOF

    echo "Using runsettings file: ${runsettings_file}"

    # Run tests
    dotnet test ${TEST_PROJECT_DIR} \
        --configuration Release \
        --no-build \
        --verbosity normal \
        --logger "trx;LogFileName=${TRX_REPORT_FILE}" \
        --settings "${runsettings_file}" \
        --results-directory "${REPORT_BASE_DIR}"

    local test_exit_code=$?

    if [ ${test_exit_code} -eq 0 ]; then
        status_message "$GREEN" "Tests successful. Processing reports..."
        
        local coverage_path="${REPORT_BASE_DIR}/${COVERAGE_REPORT_FILE}"
        local html_report_dir="${REPORT_BASE_DIR}/${HTML_REPORT_SUBDIR}"
        
        if [ -f "${coverage_path}" ]; then
            # Generate HTML report
            if install_report_generator && generate_html_report "${coverage_path}" "${html_report_dir}"; then
                local html_report_path="${html_report_dir}/${HTML_REPORT_FILE}"
                if [ -f "${html_report_path}" ]; then
                    open_report_in_browser "${html_report_path}"
                fi
            fi
        else
            status_message "$YELLOW" "Coverage file not found at expected location"
            find_and_consolidate_coverage "${coverage_path}" "${REPORT_BASE_DIR}"
        fi
        
        # Clean up and show structure
        clean_extra_directories
        show_final_structure
        
    else
        status_message "$RED" "Tests failed. Review the results."
        return 1
    fi

    return ${test_exit_code}
}

run_api() {
    status_message "$YELLOW" "Starting .NET Web API application..."
    echo -e "${YELLOW}The application will run until you press Ctrl+C.${NC}"

    if [ ! -d "${API_PROJECT_DIR}" ]; then
        status_message "$RED" "Error: API project directory not found: ${API_PROJECT_DIR}"
        return 1
    fi

    dotnet run --project ${API_PROJECT_DIR} --launch-profile "http"
}

create_solution() {
    status_message "$YELLOW" "Creating solution file and linking projects..."

    if [ -f "${SLN_FILE}" ]; then
        status_message "$YELLOW" "Solution file (${SLN_FILE}) already exists. Skipping creation."
        return 0
    fi

    status_message "$YELLOW" "Creating new solution file: ${SLN_FILE}..."
    dotnet new sln --name "$(basename ${SLN_FILE} .sln)"
    if [ $? -ne 0 ]; then
        status_message "$RED" "Failed to create solution file."
        return 1
    fi

    status_message "$YELLOW" "Adding API project (${API_PROJECT_DIR}) to solution..."
    dotnet sln ${SLN_FILE} add ${API_PROJECT_DIR}
    if [ $? -ne 0 ]; then
        status_message "$RED" "Failed to add API project."
        return 1
    fi

    status_message "$YELLOW" "Adding Test project (${TEST_PROJECT_DIR}) to solution..."
    dotnet sln ${SLN_FILE} add ${TEST_PROJECT_DIR}
    if [ $? -ne 0 ]; then
        status_message "$RED" "Failed to add Test project."
        return 1
    fi

    status_message "$GREEN" "Solution setup complete."
    return 0
}

open_report_in_browser() {
    local report_path="$1"
    status_message "$YELLOW" "Attempting to open HTML test report..."

    if [ ! -f "${report_path}" ]; then
        status_message "$RED" "Error: HTML test report not found at ${report_path}"
        return 1
    fi

    if command -v open >/dev/null 2>&1; then
        open "${report_path}"
    elif command -v xdg-open >/dev/null 2>&1; then
        xdg-open "${report_path}"
    elif command -v start >/dev/null 2>&1; then
        start "$(convert_to_win_path "${report_path}")" 
    else
        status_message "$YELLOW" "Warning: Could not automatically open report. Please view it manually at: ${report_path}"
        return 0
    fi
    status_message "$GREEN" "HTML Report opened in the default browser."
    return 0
}

regenerate_report() {
    status_message "$YELLOW" "Regenerating HTML report from existing coverage data..."
    
    local coverage_file="${REPORT_BASE_DIR}/${COVERAGE_REPORT_FILE}"
    local html_report_dir="${REPORT_BASE_DIR}/${HTML_REPORT_SUBDIR}"
    
    # First try the expected location, then search for any coverage file
    if [ ! -f "${coverage_file}" ]; then
        status_message "$YELLOW" "Coverage file not found at expected location, searching..."
        if ! find_and_consolidate_coverage "${coverage_file}" "${REPORT_BASE_DIR}"; then
            status_message "$RED" "No coverage file found. Run tests first: $0 test"
            return 1
        fi
    fi
    
    if install_report_generator && generate_html_report "${coverage_file}" "${html_report_dir}"; then
        local html_report_path="${html_report_dir}/${HTML_REPORT_FILE}"
        if [ -f "${html_report_path}" ]; then
            open_report_in_browser "${html_report_path}"
        else
            status_message "$RED" "Error: HTML report generation failed - main file not found."
            return 1
        fi
    fi
    return 0
}

# --- Main Script Execution ---

show_help() {
    echo "Usage: $0 {command}"
    echo ""
    echo "This script automates the build, test, and run cycle for a .NET Web API."
    echo ""
    echo "Commands:"
    echo "  all       : Run clean, restore, build, and test (including HTML report generation)."
    echo "  build     : Restore dependencies and build the solution (Release)."
    echo "  test      : Run tests with Coverlet (recommended - cleaner output)."
    echo "  test2     : Run tests with runsettings (alternative method)."
    echo "  run       : Build (if needed) and start the Web API application."
    echo "  clean     : Clean project build artifacts (bin/obj folders)."
    echo "  setup     : Creates the solution file and links the API and Test projects."
    echo "  report    : Generate HTML report from existing coverage data."
    echo "  help      : Show this help message."
    echo ""
    echo "Examples:"
    echo "  $0 setup    # First-time project setup"
    echo "  $0 all      # Full CI pipeline"
    echo "  $0 test     # Run tests with clean output"
    echo "  $0 report   # Regenerate HTML report"
    echo ""
    echo "NOTE: Update project paths inside the script before first use."
    echo "Current OS detected: ${OS_ENV}"
}

main() {
    case "$1" in
        all)
            check_dependencies || exit 1
            validate_configuration || exit 1
            clean_projects || exit 1
            restore_dependencies || exit 1
            build_solution || exit 1
            run_tests_coverlet
            ;;
        build)
            check_dependencies || exit 1
            validate_configuration || exit 1
            restore_dependencies || exit 1
            build_solution || exit 1
            ;;
        test)
            check_dependencies || exit 1
            validate_configuration || exit 1
            build_solution || exit 1
            run_tests_coverlet
            ;;
        test2)
            check_dependencies || exit 1
            validate_configuration || exit 1
            build_solution || exit 1
            run_tests_runsettings
            ;;
        run)
            check_dependencies || exit 1
            validate_configuration || exit 1
            build_solution || exit 1
            run_api
            ;;
        clean)
            clean_projects
            ;;
        setup)
            check_dependencies || exit 1
            create_solution
            ;;
        report)
            regenerate_report
            ;;
        help|--help|-h)
            show_help
            ;;
        *)
            show_help
            if [ "$1" != "" ]; then
                status_message "$RED" "Invalid command: $1"
            fi
            exit 1
            ;;
    esac
}

main "$@"