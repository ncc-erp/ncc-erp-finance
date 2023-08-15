import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'dropdownFilter'
})
export class DropdownFilterPipe implements PipeTransform {

  transform(value: any[], property: string, searchText: string, property2?: string, surname?: string, name?: string): any {

    // if(property2){
    //   return value.filter(item=> {
    //     let name = item[property].split(" ")
    //     return item[property].toLowerCase().includes(searchText.toLowerCase()) ||
    //      item[property2].toLowerCase().includes(searchText.toLowerCase()) || 
    //      (name[name.length-1] + ' ' + name[0]).toLowerCase().includes(searchText.toLowerCase()) 
    //   });
    // }
    // else 
    if (surname && name) {
      return value.filter(item => {
        let name = item[property].split(" ")
        return this.removeAccents(item[property].toLowerCase().replace(/\s/g, "")).includes(this.removeAccents(searchText.toLowerCase().replace(/\s/g, ""))) ||
          this.removeAccents(item[property2].toLowerCase().replace(/\s/g, "")).includes(this.removeAccents(searchText.toLowerCase().replace(/\s/g, ""))) ||
          this.removeAccents((name[name.length - 1] + name[0])).toLowerCase().replace(/\s/g, "").includes(this.removeAccents(searchText.toLowerCase().replace(/\s/g, ""))) ||
          this.removeAccents((item?.surname.toLowerCase().replace(/\s/g, "") + item?.name.toLowerCase().replace(/\s/g, ""))).includes(this.removeAccents(searchText.toLowerCase().replace(/\s/g, "")))
      });
    }
    else {
      if (value) {
        return value?.filter(item => {
          return this.removeAccents(item[property]?.toLowerCase().replace(/\s/g, "")).includes(this.removeAccents(searchText?.toLowerCase().replace(/\s/g, ""))) ||
          this.removeAccents(item[property2]?.toLowerCase().replace(/\s/g, "")).includes(this.removeAccents(searchText?.toLowerCase().replace(/\s/g, ""))) 
          
        });
      }

    }
  }
  removeAccents(str) {
    var AccentsMap = [
      "aàảãáạăằẳẵắặâầẩẫấậ",
      "AÀẢÃÁẠĂẰẲẴẮẶÂẦẨẪẤẬ",
      "dđ", "DĐ",
      "eèẻẽéẹêềểễếệ",
      "EÈẺẼÉẸÊỀỂỄẾỆ",
      "iìỉĩíị",
      "IÌỈĨÍỊ",
      "oòỏõóọôồổỗốộơờởỡớợ",
      "OÒỎÕÓỌÔỒỔỖỐỘƠỜỞỠỚỢ",
      "uùủũúụưừửữứự",
      "UÙỦŨÚỤƯỪỬỮỨỰ",
      "yỳỷỹýỵ",
      "YỲỶỸÝỴ"
    ];
    for (var i = 0; i < AccentsMap.length; i++) {
      var re = new RegExp('[' + AccentsMap[i].substr(1) + ']', 'g');
      var char = AccentsMap[i][0];
      str = str.replace(re, char);
    }
    return str;
  }
}
