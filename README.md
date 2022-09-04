# TestAutomationVMAPI

To Run Test Cases:
1. clone below repos on local:

    https://github.com/koshaljain5/VMPoolTest_API
      - This is the codebase of VM Pool Server

    https://github.com/koshaljain5/TestAutomation_VM_API
     - This is the codebase of API Automated Tests
     
2. Run Solution of VM Pool Server and test the swagger url
    https://localhost:7022/swagger/index.html
    
3. Now Open Solution API Automated Tests
    Run the Test Suite
    All Tests should get pass
    
Test Cases Details: Coverage is ~100% as per the requirement specs

TC-1 Checkout a VM from VM Pool
 * User successfully Checkout in VM
 * Returns Gets IP address of VM
 
TC-2 Checkin from VM, clean up VM reservation details
 * User successfully Checkin from VM
 * Returns: VM Usage time
 * free VM in VMPool
 
TC-3 Negative Case: Checkout a VM, When Already reserved 1 with same username

TC-4 Negative Case: Checkin from VM when No Prior Checkout to any VM

TC-5 Negative Case: Checkout to VM with Invalid User Name

TC-6 Negative Case: Checkout to VM when VM Pool is full with other reservations


