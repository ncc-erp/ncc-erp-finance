import { AppConsts, OPTION_ALL } from '../../../shared/AppConsts';
export class Utils {
    static isSelectedOptionsAll(value: string | number | boolean): boolean {
        if (value === AppConsts.VALUE_OPTIONS_ALL || value === null || value === OPTION_ALL)
            return true;
        return false;
    }

    static toNumber(value: any){
        return Number(value)
    }
}
