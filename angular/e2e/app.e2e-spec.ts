import { FinanceManagementTemplatePage } from './app.po';

describe('FinanceManagement App', function() {
  let page: FinanceManagementTemplatePage;

  beforeEach(() => {
    page = new FinanceManagementTemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
