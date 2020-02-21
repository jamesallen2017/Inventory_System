/* 

    App Name    : Bycicle - Responsive Admin Theme
    Author      : Pixelfarm.in
    Author URI  : http://Pixelfarm.in/

*/

var FormValidationInline = function (){
    
    var handelFormValidation = function() {
        
        // validate signup form on keyup and submit
	$(".form-validate").validate({
            errorElement: 'span', //default input error message container
            errorClass: 'error', // default input error message class
            focusInvalid: false, // do not focus the last invalid input
            ignore: "",
            
            rules: {
                CompanyID: "required",
                Username: "required",
                RoleID: "required",
                GroupID: "required",
                Fullname: "required",
                DepartmentID: "required",
                DesignationID: "required",
                
                EmailAddress: {
                    required: true,
                    email: true
                },
            },
            
            messages: {
                firstname: "Please enter the First Name",
                RackNo: "Please enter Rack No.",
                RackDescription: "Please enter the Description",
                lastname: "Please enter the Last Name",
                email: "Please enter Correct E-mail Address",
            },
            highlight: function (element) { // hightlight error inputs
                $(element)
                .closest('.form-group').addClass('has-error'); // set error class to the control group
            },
            unhighlight: function (element) { // revert the change done by hightlight
                $(element)
                .closest('.form-group').removeClass('has-error'); // set error class to the control group
            }
	});
    };
    
    return {
        init: function() {
            handelFormValidation();
        }
        
    };
}();    // Handel Form Validation

var FormValidationTooltip = function (){
    
    var handelFormValidation = function() {
        $("#formvalidationtooltip").validationEngine({ promptPosition: "topLeft" });
        $(".ItemLocation").validationEngine({ promptPosition: "topLeft" });
    };
    
    return {
        init: function() {
            handelFormValidation();
        }
        
    };
}();    // Handel Form Validation

