import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import {
  Router,
  RouterEvent,
  NavigationEnd,
  PRIMARY_OUTLET
} from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { filter } from 'rxjs/operators';
import { MenuItem } from '@shared/layout/menu-item';

@Component({
  selector: 'sidebar-menu',
  templateUrl: './sidebar-menu.component.html'
})
export class SidebarMenuComponent extends AppComponentBase implements OnInit {
  menuItems: MenuItem[];
  menuItemsMap: { [key: number]: MenuItem } = {};
  activatedMenuItems: MenuItem[] = [];
  routerEvents: BehaviorSubject<RouterEvent> = new BehaviorSubject(undefined);
  homeRoute = '/app/home';

  constructor(injector: Injector, private router: Router) {
    super(injector);
    this.router.events.subscribe(this.routerEvents);
  }

  ngOnInit(): void {
    this.menuItems = this.getMenuItems();
    this.patchMenuItems(this.menuItems);
    this.routerEvents
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe((event) => {
        const currentUrl = event.url !== '/' ? event.url : this.homeRoute;
        const primaryUrlSegmentGroup = this.router.parseUrl(currentUrl).root
          .children[PRIMARY_OUTLET];
        if (primaryUrlSegmentGroup) {
          this.activateMenuItems('/' + primaryUrlSegmentGroup.toString());
        }
      });
  }

  getMenuItems(): MenuItem[] {
    return [
      new MenuItem('menu.menu1', '/app/home', 'fas fa-home', 'Dashboard'),
      new MenuItem('menu.menu2', '', 'fas fa-user-cog', 'Admin', [
        new MenuItem(
          'menu2.tenants',
          '/app/tenants',
          'fas fa-building',
          'Admin.Tenant'
        ),
        new MenuItem(
          'menu2.user',
          '/app/users',
          'fas fa-users',
          'Admin.User'
        ),
        new MenuItem(
          'menu2.role',
          '/app/roles',
          'fas fa-user-tag',
          'Admin.Role'
        ),
        new MenuItem(
          'menu2.workFlow',
          '/app/workFlow',
          'fas fa-briefcase',
          'Admin.Workflow'
        ),
        new MenuItem(
          'menu2.status',
          '/app/status',
          'fas fa-star',
          'Admin.WorkflowStatus'
        ),
        new MenuItem(
          'menu2.configuration',
          '/app/setting',
          'fas fa-cog',
          'Admin.Configuration'
        ),
        new MenuItem(
          'Chart setting',
          '/app/lineChartSetting',
          'fas fa-cogs',
          'Admin.LineChartSetting'
        ),
        new MenuItem(
          'Lịch sử crawl giao dịch',
          '/app/b-transaction-log',
          'fas fa-history',
          'Admin.CrawlHistory'
        ),
        new MenuItem(
          'Audit Log',
          '/app/auditlog',
          'fa fa-wrench',
          'Admin.Auditlog'
        ),
      ]),
      new MenuItem('menu.menu3', '', 'fas fa-folder', 'Directory', [
        new MenuItem(
          'menu3.m3_child1',
          '/app/currencies',
          'fas fa-euro-sign',
          'Directory.Currency'
        ),
        new MenuItem(
          'Tỉ giá',
          '/app/currency-convert',
          'fas fa-dollar-sign',
          'Directory.CurrencyConvert'
        ),
        new MenuItem(
          'menu3.m3_child2',
          '/app/banks',
          'fas fa-university',
          'Directory.Bank'
        ),
        new MenuItem(
          'menu3.m3_child3',
          '/app/accountType',
          'fas fa-user-circle',
          'Directory.AccountType'
        ),
        new MenuItem(
          'menu3.m3_child4',
          '/app/branches',
          'fas fa-code-branch',
          'Directory.Branch'
        ),
        new MenuItem(
          'menu3.m3_child5',
          '/app/incomingType',
          'fas fa-indent',
          'Directory.IncomingEntryType'
        ),
        new MenuItem(
          'menu3.m3_child6',
          '/app/outcomingType',
          'fas fa-outdent',
          'Directory.OutcomingEntryType'
        ),
        new MenuItem(
          'menu3.m3_child7',
          '/app/supplierList',
          'fas fa-truck-moving',
          'Directory.Supplier'
        )
      ]
      ),
      new MenuItem('menu.menu4', '', 'fas fa-id-card', 'Account.Directory', [
        new MenuItem(
          'menu4.m4_child1',
          '/app/bank-account',
          'fas fa-money-check-alt',
          'Account.Directory.BankAccount'
        ),
        new MenuItem(
          'menu4.m4_child2',
          '/app/accountant-account',
          'fas fa-money-check',
          'Account.Directory.FinanceAccount'
        )
      ]),
      new MenuItem('menu.menu5', '', 'fas fa-funnel-dollar', 'Finance', [
        new MenuItem(
          'menu5.m5_child1',
          '/app/revenue-record',
          'fas fa-pen-square',
          'Finance.IncomingEntry'
        ),
        new MenuItem(
          'menu5.m5_child2',
          '/app/expenditure-request',
          'fas fa-comment-dollar',
          'Finance.OutcomingEntry'
        ),
        new MenuItem(
          'menu5.m5_child3',
          '/app/bank-transaction',
          'fas fa-search-dollar',
          'Finance.BankTransaction'
        ),
        // new MenuItem(
        //   'm5_child5.title',
        //   '/app/revenue-managed',
        //   'fas fa-money-bill-wave',
        //   'Finance.RevenueManaged'
        // ),
        // new MenuItem(
        //   'menu5.m5_child4',
        //   '/app/invoice',
        //   'fas fa-file-invoice-dollar',
        //   "Finance.Invoice"
        // ),
        new MenuItem(
          'Đối soát mới',
          '/app/finance-review',
          'fas fa-search',
          "Finance.ComparativeStatisticNew"
        ),
        new MenuItem(
          'Đối soát',
          '/app/finance-statistic-old',
          'fas fa-search',
          "Finance.ComparativeStatistic"
        ),

        new MenuItem(
          'Biến động số dư',
          '/app/btransaction',
          'fas fa-dollar-sign',
          'Finance.BĐSD'
        ),

        new MenuItem(
          'm5_child5.title',
          '/app/nrevenue',
          'fas fa-file-invoice-dollar',
          'Finance.Invoice'
        ),
        new MenuItem(
          'Kỳ kế toán',
          '/app/period',
          'fas fa-business-time',
          'Finance.Period'
        )
      ]),
    ];
  }

  patchMenuItems(items: MenuItem[], parentId?: number): void {
    items.forEach((item: MenuItem, index: number) => {
      item.id = parentId ? Number(parentId + '' + (index + 1)) : index + 1;
      if (parentId) {
        item.parentId = parentId;
      }
      if (parentId || item.children) {
        this.menuItemsMap[item.id] = item;
      }
      if (item.children) {
        this.patchMenuItems(item.children, item.id);
      }
    });
  }

  activateMenuItems(url: string): void {
    this.deactivateMenuItems(this.menuItems);
    this.activatedMenuItems = [];
    const foundedItems = this.findMenuItemsByUrl(url, this.menuItems);
    foundedItems.forEach((item) => {
      this.activateMenuItem(item);
    });
  }

  deactivateMenuItems(items: MenuItem[]): void {
    items.forEach((item: MenuItem) => {
      item.isActive = false;
      item.isCollapsed = true;
      if (item.children) {
        this.deactivateMenuItems(item.children);
      }
    });
  }

  findMenuItemsByUrl(
    url: string,
    items: MenuItem[],
    foundedItems: MenuItem[] = []
  ): MenuItem[] {
    items.forEach((item: MenuItem) => {
      if (item.route === url) {
        foundedItems.push(item);
      } else if (item.children) {
        this.findMenuItemsByUrl(url, item.children, foundedItems);
      }
    });
    return foundedItems;
  }

  activateMenuItem(item: MenuItem): void {
    item.isActive = true;
    if (item.children) {
      item.isCollapsed = false;
    }
    this.activatedMenuItems.push(item);
    if (item.parentId) {
      this.activateMenuItem(this.menuItemsMap[item.parentId]);
    }
  }

  isMenuItemVisible(item: MenuItem): boolean {
    if (!item.permissionName) {
      return true;
    }
    return this.permission.isGranted(item.permissionName);
  }
}
